using System;
using System.Collections.Generic;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public partial class Camp : MonoBehaviour
    {
        [SerializeField]
        private ECamp m_CurCamp = ECamp.Green;
        public ECamp CurCamp
        {
            get { return m_CurCamp; }
            set
            {
                if (m_CurCamp != value)
                {
                    m_CurCamp = value;
                    UpdateCampSprite();
                    OnCampChange?.InvokeSafe(m_CurCamp);
                }
            }
        }

        public List<Sprite> sprites = new();

        public List<GameObject> prefabs = new();

        public event Action<ECamp> OnCampChange;

        public bool IsBroken
        {
            get
            {
                if (!Application.isPlaying)
                    return false;
                if (!TryGetComponent(out UnitHealth unitHealth))
                    return false;
                return unitHealth.IsDead;
            }
        }

        private void UpdateCampSprite()
        {
            var oldSprite = transform.Find("Sprite");
            if (oldSprite)
                DestroyImmediate(oldSprite.gameObject);

            string nameSuffix = "_" + CurCamp.ToString().ToLower();
            string brokenSuffix = "_broken" + nameSuffix;
            bool isBroken = IsBroken;

            // 切换模型
            foreach (var obj in prefabs)
            {
                if (!obj.name.ToLower().EndsWith(nameSuffix))
                    continue;
                bool isBrokenSprite = obj.name.ToLower().EndsWith(brokenSuffix);
                if (isBroken == isBrokenSprite)
                {
                    GameObject go = Instantiate(obj, transform);
                    go.name = "Sprite";
                    return;
                }
            }

            // 切换贴图
            foreach (var sprite in sprites)
            {
                if (!sprite.name.ToLower().EndsWith(nameSuffix))
                    continue;
                bool isBrokenSprite = sprite.name.ToLower().EndsWith(brokenSuffix);
                if (isBroken == isBrokenSprite)
                {
                    var spriteRenderer = gameObject.GFGetOrAddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                    return;
                }
            }
        }

        void Start()
        {
            UpdateCampSprite();
            OnCampChange?.InvokeSafe(m_CurCamp);
        }

        void OnDestroy()
        {
            OnCampChange = null;
        }


#if UNITY_EDITOR
        private void SelectSameCampGameObject()
        {
            var cur = gameObject.GetComponent<Camp>().CurCamp;
            var camps = GameObject.FindObjectsByType<Camp>(FindObjectsSortMode.None);
            var selected = new List<UnityEngine.Object>();
            foreach (var camp in camps)
            {
                if (camp.CurCamp == cur)
                    selected.Add(camp.gameObject);
            }
            UnityEditor.Selection.objects = selected.ToArray();
        }
#endif
    }
}