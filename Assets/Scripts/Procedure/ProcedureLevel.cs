using GameFramework.AOT;
using GameFramework.Hot;

namespace Takeover
{
    public class ProcedureLevel : ProcedureBase
    {
        protected override void OnEnter(object userData)
        {
            base.OnEnter(userData);
            Log.Error("关卡" + userData);
        }
    }
}