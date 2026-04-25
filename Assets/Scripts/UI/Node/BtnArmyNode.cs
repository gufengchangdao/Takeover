using GameFramework.Hot;
using TMPro;

namespace Takeover
{
    public class BtnArmyNode : BaseUINode
    {
        public GFImage imgBg;
        public GFImage imgIcon;
        public TextMeshProUGUI txtPrice;
        public GFButton btn;


        public override void OnInit()
        {
            base.OnInit();
            foreach (var camp in GetComponentsInChildren<Camp>())
                camp.CurCamp = Global.CombotantData.Camp;

            Gray = false;
        }

        public void Init(string atlasPath, string imageName, int price)
        {
            imgIcon.SetImageAsync(atlasPath, imageName);
            txtPrice.text = price + "";
        }

        public override void OnRecycle()
        {
            btn.onClick.Clear();
            base.OnRecycle();
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
                    imgBg.SetGray(value);
                    imgIcon.SetGray(value);
                }
            }
        }
    }
}