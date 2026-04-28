using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;
using UnityEngine.U2D;

namespace Takeover
{
    public class ArmyHealthBar : UpdateableComponent
    {
        public const float HEALTH_BAR_HEIGHT = 1;
        private const float ICON_SPACING = 0.1f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Camp camp;
        [SerializeField] private SpriteRenderer imgIcon;

        private Army army;

        private static int SHADER_CLIP_ID = Shader.PropertyToID("_ClipUvUp");

        protected override void Start()
        {
            base.Start();
            army = GetComponentInParent<Army>();
            camp.CurCamp = army.Camp;
            var atlas = GFGlobal.Resource.LoadAssetSync<SpriteAtlas>(GFGlobal.Tables.TbGlobalSettingData.ArmyIconPath);
            imgIcon.sprite = atlas.GetSprite(army.TableId);
            OnLateUpdate(0);
        }

        public void SetHealthPercent(float percent)
        {
            spriteRenderer.material.SetFloat(SHADER_CLIP_ID, percent);
        }

        private float lastHealthPercent = -1;

        // 更新血量和位置
        public override void OnLateUpdate(float dt)
        {
            float healthPercent = army.HealthPercent;
            Castle castle = army.CurCastle;
            var units = army.Units;

            if (!Mathf.Approximately(lastHealthPercent, healthPercent))
            {
                lastHealthPercent = healthPercent;
                SetHealthPercent(1 - healthPercent);
            }

            if (castle)
            {
                int index = castle.Defenders.IndexOf(army);
                var anchor = castle.NodeMap.GetTransform("ArmyIconPos").transform.position;
                float width = spriteRenderer.bounds.size.x;
                int count = castle.Defenders.Count;
                float totalWidth = count * width + (count - 1) * ICON_SPACING;
                float leftX = anchor.x - totalWidth / 2 + width / 2;
                float x = leftX + index * (width + ICON_SPACING);
                transform.position = new Vector2(x, anchor.y);
            }
            else
            {
                float x = 0, y = 0;
                int count = 0;
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (!unit.Health.IsDead)
                    {
                        var pos = unit.Health.transform.position;
                        x += pos.x;
                        y += pos.y;
                        count++;
                    }
                }
                transform.position = new Vector2(x / count, y / count + HEALTH_BAR_HEIGHT);
            }
        }
    }
}
