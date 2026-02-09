using System;
using GameFramework.Hot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Takeover
{
    [Serializable]
    public class BtnArmyNode : BaseUINode
    {
        public Image imgIcon;
        public TextMeshProUGUI txtPrice;
        public GFButton btn;

        private bool m_isGray = false;
        public bool IsGray
        {
            get
            {
                return m_isGray;
            }
            set
            {
                m_isGray = value;
            }
        }
    }
}