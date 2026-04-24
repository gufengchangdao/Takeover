using System.Collections.Generic;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public class ArmyHealthBar : MonoBehaviour
    {
        public const int HEALTH_BAR_HEIGHT = 80;
        public const int HEALTH_BAR_CASTLE_HEIGHT = HEALTH_BAR_HEIGHT + 20;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Camp camp;

        private static int SHADER_CLIP_ID = Shader.PropertyToID("_ClipUvUp");

        void Awake()
        {
            spriteRenderer.material = new Material(spriteRenderer.sharedMaterial);
        }

        public void SetCamp(ECamp camp)
        {
            this.camp.CurCamp = camp;
        }

        public void SetHealthPercent(float percent)
        {
            spriteRenderer.material.SetFloat(SHADER_CLIP_ID, percent);
        }

        // 更新血量和位置
        public void UpdateHealthAndPosition(List<Unit> units, bool inCastle)
        {
            int totalMax = 0;
            int totalCur = 0;
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                totalMax += unit.Health.MaxHealth;
                totalCur += unit.Health.CurHealth;
            }
            SetHealthPercent(totalCur * 1f / totalMax);

            int height = inCastle ? HEALTH_BAR_CASTLE_HEIGHT : HEALTH_BAR_HEIGHT;
        }
    }
}