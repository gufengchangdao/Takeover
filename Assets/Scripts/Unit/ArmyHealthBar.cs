using System.Collections.Generic;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public class ArmyHealthBar : MonoBehaviour
    {
        public const float HEALTH_BAR_HEIGHT = 1;
        public const float HEALTH_BAR_CASTLE_HEIGHT = HEALTH_BAR_HEIGHT + 0.2f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Camp camp;
        [SerializeField] private SpriteRenderer imgIcon;

        private static int SHADER_CLIP_ID = Shader.PropertyToID("_ClipUvUp");

        public void Init(ECamp camp, Sprite icon)
        {
            this.camp.CurCamp = camp;
            imgIcon.sprite = icon;
        }

        public void SetHealthPercent(float percent)
        {
            spriteRenderer.material.SetFloat(SHADER_CLIP_ID, percent);
        }

        private float lastHealthPercent = -1;

        // 更新血量和位置
        public void UpdateHealthAndPosition(List<Unit> units, float healthPercent, bool inCastle)
        {
            if (!Mathf.Approximately(lastHealthPercent, healthPercent))
            {
                lastHealthPercent = healthPercent;
                SetHealthPercent(1 - healthPercent);
            }

            float height = inCastle ? HEALTH_BAR_CASTLE_HEIGHT : HEALTH_BAR_HEIGHT;
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
            transform.position = new Vector2(x / count, y / count + height);
        }
    }
}