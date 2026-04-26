using Takeover;
using TMPro;
using UnityEngine;

namespace CastleHealthBar
{
    public class CastleHealthBar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer srBar;
        [SerializeField] private SpriteRenderer srShield;
        [SerializeField] private TextMeshPro txtLevel;

        private Castle castle;
        private static readonly int SHADER_PARAM_CLIP = Shader.PropertyToID("_ClipUvRight");

        void Start()
        {
            castle = GetComponentInParent<Castle>();
            var color = ColorEnum.GetCampColor(castle.Camp);

            srBar.color = color;
            srShield.color = color;

            OnLevelChange();
            OnHealthChange(castle.Health.CurHealth, castle.Health.CurHealth);

            castle.Level.AddOnChange(OnLevelChange);
            castle.Health.OnHealthChange += OnHealthChange;
        }

        private void OnLevelChange()
        {
            txtLevel.text = ((castle.Level.Value + 1) * 5) + "";
        }

        private void OnHealthChange(int cur, int old)
        {
            int maxHealth = castle.Health.MaxHealth;
            srBar.material.SetFloat(SHADER_PARAM_CLIP, 1 - cur / maxHealth);
        }
    }
}