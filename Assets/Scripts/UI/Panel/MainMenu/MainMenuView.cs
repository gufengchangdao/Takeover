using GameFramework.Hot;

namespace Takeover
{
    public class MainMenuView : BaseView<MainMenuControl>
    {
        public GFButton btnStart;
        public GFButton btnExit;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            BtnOnClick(btnStart, e =>
            {
                GFGlobal.UI.OpenPanel<CampaignSelectControl>();
                Close();
            });
            BtnOnClick(btnExit, e =>
            {
                Global.Quit();
            });
        }
    }
}