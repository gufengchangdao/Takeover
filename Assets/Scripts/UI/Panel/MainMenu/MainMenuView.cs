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
            btnStart.onClick.AddEventListener((e) =>
            {
                Close();
            });
            btnExit.onClick.AddEventListener((e) =>
            {
                Global.Quit();
            });
        }
    }
}