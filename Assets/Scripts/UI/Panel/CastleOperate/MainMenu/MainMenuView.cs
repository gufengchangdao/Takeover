using GameFramework.Hot;

namespace Takeover
{
    public class MainMenuView : BaseView<MainMenuControl>
    {
        public GFButton btnStart;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            btnStart.onClick.AddEventListener((e) =>
            {
                Close();
            });
        }
    }
}