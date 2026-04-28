using GameFramework.AOT;
using UnityEngine;
using YooAsset;

namespace GameFramework.Hot
{
    [DisallowMultipleComponent]
    public class GFGlobal : MonoBehaviour
    {
        public static GameConfig Config { get; private set; }
        public static GFSave Save { get; private set; }
        public static GFReferencePool ReferencePool { get; private set; }
        public static GFEvent Event { get; private set; }
        public static GFTimer Timer { get; private set; }
        public static GFResource Resource { get; private set; }
        public static GFDataTable DataTable { get; private set; }
#if USE_LUBAN
        public static TableStructure.Tables Tables => DataTable.Tables; //直接拿出来，方便获取
        public static TableStructure.TbGlobalSettingData GlobalTableData => Tables.TbGlobalSettingData;
#endif
        public static GFInput Input { get; private set; }
        public static GFFsm Fsm { get; private set; }
        public static GFProcedure Procedure { get; private set; }
        public static GFBehaviorTree BehaviorTree { get; private set; }
        public static GFUI UI { get; private set; }
        public static Camera UICamera => UI.UICamera;
        public static GFSound Sound { get; private set; }
        public static GFScene Scene { get; internal set; }
        public static GFCamera Camera { get; internal set; }
        // public static GFEntity Entity { get; private set; }

        void Awake()
        {
            Log.Info("Start init GameFramework");

            // 语言
            AOTGameConfig.Language = LanguageUtility.GetSupportedLanguage(Application.systemLanguage);

            // 加载配置
            string configPath = "Assets/Content/Config/GameConfig.asset";
            var package = YooAssets.GetPackage(GameConfig.DefaultPackage);
            Config = package.LoadAssetSync<GameConfig>(configPath).GetAssetObject<GameConfig>();
            if (Config == null)
            {
                Log.Error("config file not found.");
                return;
            }

            InitModuleComponents();

            // 加载字体
            GFFontManager.Instance.Init();
            AOTGameConfig.OnLanguageChange += OnLanguageChange;


            Log.Info("GameFramework init success!");
        }

        void Update()
        {
            Save.ModuleUpdate();
            ReferencePool.ModuleUpdate();
            Event.ModuleUpdate();
            Timer.ModuleUpdate();
            Resource.ModuleUpdate();
            DataTable.ModuleUpdate();
            Input.ModuleUpdate();
            Fsm.ModuleUpdate();
            Procedure.ModuleUpdate();
            BehaviorTree.ModuleUpdate();
            UI.ModuleUpdate();
            Sound.ModuleUpdate();
            Scene.ModuleUpdate();
            Camera.ModuleUpdate();
        }

        private void InitModuleComponents()
        {
            Save = AddModuleComponent<GFSave>("Save");
            ReferencePool = AddModuleComponent<GFReferencePool>("ReferencePool");
            Event = AddModuleComponent<GFEvent>("Event");
            Timer = AddModuleComponent<GFTimer>("Timer");
            Resource = AddModuleComponent<GFResource>("Resource");
            DataTable = AddModuleComponent<GFDataTable>("DataTable");
            Input = AddModuleComponent<GFInput>("Input");
            Fsm = AddModuleComponent<GFFsm>("Fsm");
            Procedure = AddModuleComponent<GFProcedure>("Procedure");
            BehaviorTree = AddModuleComponent<GFBehaviorTree>("BehaviorTree");
            UI = AddModuleComponent<GFUI>("UI");
            Sound = AddModuleComponent<GFSound>("Sound");
            Scene = AddModuleComponent<GFScene>("Scene");
            Camera = AddModuleComponent<GFCamera>("Camera");
        }

        private T AddModuleComponent<T>(string name) where T : Component
        {
            Log.Info("Add module component: {0}", name);
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            return go.AddComponent<T>();
        }

        private void OnLanguageChange(SystemLanguage old)
        {
            var texts = GameObject.FindObjectsByType<GFText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var text in texts)
                text.OnLanguageChange();
        }
    }
}