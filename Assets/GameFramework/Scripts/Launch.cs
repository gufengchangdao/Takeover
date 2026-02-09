using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using GameFramework.AOT;
#if USE_HYBRIDCLR
using HybridCLR;
#endif
using UnityEngine;
using YooAsset;

namespace GameFramework.AOT
{
    [DisallowMultipleComponent]
    public class Launch : MonoBehaviour
    {
        public static Launch Instance { get; private set; }

        public const string DefaultPackage = "DefaultPackage";

        private EPlayMode playMode = EPlayMode.EditorSimulateMode;

        public string defaultPackageVersion;
        private const string DataTableDllName = "DataTable";
        private const string FrameworkDllName = "GameFramework.Hot";
#if USE_HYBRIDCLR
        private const string ProjectDllName = "HotUpdate";
#else
        private const string ProjectDllName = "Assembly-CSharp";
#endif

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }

        IEnumerator Start()
        {
            // 配置数据
            AOTGameConfig.PlayMode = playMode;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            DontDestroyOnLoad(gameObject);

            // 开始补丁更新流程
            YooAssets.Initialize(new ILog());
            var operation = new PatchOperation(this, DefaultPackage, defaultPackageVersion, playMode, "http://127.0.0.1/CDN/PC/v1.0");
            YooAssets.StartOperation(operation);
            yield return operation;

            YooAssets.SetDefaultPackage(YooAssets.TryGetPackage(DefaultPackage));

            if (playMode != EPlayMode.EditorSimulateMode)
                LoadMetadataForAOTAssembies();
            yield return InitHotUpdateDll();
        }

        protected static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());
        }

        private void LoadMetadataForAOTAssembies()
        {
#if USE_HYBRIDCLR
            foreach (var aotDllName in AOTGenericReferences.PatchedAOTAssemblyList)
            {
                byte[] dllBytes = File.ReadAllBytes($"{Application.streamingAssetsPath}/AOTDLL/{aotDllName}.bytes");
                var err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                Log.Info($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
#endif
        }

        private IEnumerator InitHotUpdateDll()
        {
            // 加载表格类程序集
            AssemblyInfo assemblyInfo = new();

#if USE_LUBAN
            assemblyInfo.Reset(DataTableDllName);
            yield return LoadAssemblie(assemblyInfo);
#endif
            // 加载框架程序集
            assemblyInfo.Reset(FrameworkDllName);
            yield return LoadAssemblie(assemblyInfo);
            Type globalType = assemblyInfo.assembly.GetType("GameFramework.Hot.GFGlobal");
            gameObject.AddComponent(globalType);

            // 加载业务逻辑程序集
            assemblyInfo.Reset(ProjectDllName);
            yield return LoadAssemblie(assemblyInfo);
            assemblyInfo.assembly.GetType("Global").GetMethod("Main").Invoke(null, null);
        }

        private IEnumerator LoadAssemblie(AssemblyInfo info)
        {
            Log.Info("Start Load Assemblie: {0}", info.assemblyName);
            // Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/{hotUpdateDllName}.dll.bytes"));
#if UNITY_EDITOR || USE_HYBRIDCLR
            // Editor下无需加载，直接查找获得HotUpdate程序集
            // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
            info.assembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == info.assemblyName);
#else
            var handle = YooAssets.LoadAssetAsync<TextAsset>($"Assets/Content/HotUpdateDLL/{info.assemblyName}.dll.bytes");
            yield return handle;
            info.assembly = Assembly.Load(((TextAsset)handle.AssetObject).bytes);
#endif

            Log.Info("Load Assemblie success: {0}", info.assemblyName);
            yield break;
        }


        private class AssemblyInfo
        {
            public string assemblyName;
            public Assembly assembly;
            public void Reset(string assemblyName)
            {
                this.assemblyName = assemblyName;
                assembly = null;
            }
        }
    }
}