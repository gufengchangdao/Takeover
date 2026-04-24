using TableStructure;
using UnityEngine;

namespace Takeover
{
    public class ArmyHealthBar : MonoBehaviour
    {
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
    }
}