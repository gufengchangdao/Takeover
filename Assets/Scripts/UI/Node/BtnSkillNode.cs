using GameFramework.Hot;

namespace Takeover
{
    public class BtnSkillNode : BaseUINode
    {
        public GFButton btn;
        public GFImage imgIcon;
        public GFImage imgFrame;
        public GFText txtCost;

        private int cost;

        public void Init(string atlasPath, string imgName, int cost)
        {
            imgIcon.SetImageAsync(atlasPath, imgName);
            this.cost = cost;
        }

        public override void OnRecycle()
        {
            btn.onClick.Clear();
            base.OnRecycle();
        }

        public void OnManaChange(float mana)
        {
            bool canSpll = mana >= cost;
            imgIcon.SetGray(!canSpll);
            imgFrame.SetGray(!canSpll);
            btn.enabled = canSpll;
        }
    }
}