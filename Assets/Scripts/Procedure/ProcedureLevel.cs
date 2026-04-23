using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;

namespace Takeover
{
    public class ProcedureLevel : ProcedureBase
    {
        protected override void OnEnter(object userData)
        {
            base.OnEnter(userData);

            GFGlobal.Event.Subscribe<SceneLoadEndEvent>(OnSceneLoaded);

            int levelId = (int)userData;
            string assetPath = string.Format(GFGlobal.Tables.TbGlobalSettingData.LevelScenePath, levelId);
            Log.Info("加载关卡：" + assetPath);
            GFGlobal.Scene.LoadSceneByPackage(assetPath);
        }

        protected override void OnLeave()
        {
            GFGlobal.Event.Unsubscribe<SceneLoadEndEvent>(OnSceneLoaded);

            GFGlobal.UI.ClosePanel<LevelControl>();

            Global.LevelLogic = null;

            base.OnLeave();
        }

        private void OnSceneLoaded(object sender, SceneLoadEndEvent data)
        {
            var go = new GameObject(nameof(LevelLogic));
            Global.LevelLogic = go.AddComponent<LevelLogic>();

            GFGlobal.UI.OpenPanel<LevelControl>();
        }
    }
}