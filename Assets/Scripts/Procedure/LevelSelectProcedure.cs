using GameFramework.Hot;

namespace Takeover
{
    public class LevelSelectProcedure : ProcedureBase
    {
        protected override void OnEnter(object userData)
        {
            base.OnEnter(userData);
            GFGlobal.Scene.LoadScene($"LevelSelect{Global.LevelData.Camp}");
        }
    }
}