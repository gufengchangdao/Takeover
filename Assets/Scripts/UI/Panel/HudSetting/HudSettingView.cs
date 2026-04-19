using GameFramework.Hot;

namespace Takeover
{
    public class HudSettingView : BaseView<HudSettingControl>
    {
        public GFButton btnSetting;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            btnSetting.onClick.AddEventListener((e) =>
            {
                if (!GFGlobal.UI.HasPanel<SettingControl>())
                    GFGlobal.UI.OpenPanel<SettingControl>();
            });
        }
    }
}