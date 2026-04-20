using GameFramework.Hot;

namespace Takeover
{
    public class ProcedureStart : ProcedureBase
    {
        protected override void OnEnter(object userData)
        {
            base.OnEnter(userData);
            GFGlobal.UI.OpenPanel<MainMenuControl>();
        }
    }
}