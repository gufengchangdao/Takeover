using System;
using GameFramework.AOT;
using GameFramework.Hot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Takeover
{
    public class BtnArmyNode : BaseUINode
    {
        public Image imgIcon;
        public TextMeshProUGUI txtPrice;
        public GFButton btn;

        private Material mat;

        public override void OnInit()
        {
            base.OnInit();

            if (!mat)
            {
                mat = GFGlobal.Resource.LoadAssetSync<Material>("Assets/Content/UI/Material/CommonUIMat.mat");
                mat = new Material(mat);
                GetComponent<Image>().material = mat;
                imgIcon.material = mat;
            }
        }

        private bool m_Gray = false;
        public bool Gray
        {
            get
            {
                return m_Gray;
            }
            set
            {
                if (m_Gray != value)
                {
                    m_Gray = value;
                    mat.SetFloat("_GreyscaleBlend", value ? 1 : 0);
                }
            }
        }
    }
}