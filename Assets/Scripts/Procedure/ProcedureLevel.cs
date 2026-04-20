using GameFramework.AOT;
using GameFramework.Hot;

namespace Takeover
{
    public class ProcedureLevel : ProcedureBase
    {
        protected override void OnEnter(object userData)
        {
            base.OnEnter(userData);
            int levelId = (int)userData;
            string assetPath = string.Format(GFGlobal.Tables.TbGlobalSettingData.LevelScenePath, levelId);
            Log.Info("加载关卡：" + assetPath);
            GFGlobal.Scene.LoadSceneByPackage(assetPath);
        }
    }
}