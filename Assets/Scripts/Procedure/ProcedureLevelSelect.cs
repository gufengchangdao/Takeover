using GameFramework.Hot;
using UnityEngine.SceneManagement;

namespace Takeover
{
    public class ProcedureLevelSelect : ProcedureBase
    {
        protected override void OnEnter(object userData)
        {
            base.OnEnter(userData);
            GFGlobal.Scene.LoadScene($"LevelSelect{Global.LevelData.Camp}");
            if (!GFGlobal.UI.HasPanel<LevelSelectControl>())
                GFGlobal.UI.OpenPanel<LevelSelectControl>();
        }

        protected override void OnLeave()
        {
            // GFGlobal.Scene.UnloadSceneAsync($"LevelSelect{Global.LevelData.Camp}");
            GFGlobal.Scene.LoadScene("Empty");
            GFGlobal.UI.ClosePanel<LevelSelectControl>();
            base.OnLeave();
        }
    }
}