using System;
using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YooAsset;

namespace GameFramework.Hot
{
    /// <summary>
    /// TODO 缓存这一块先不做，我还不知道怎么规划
    /// </summary>
    public class GFResource : GFBaseModule
    {
        private ResourcePackage defaultPackage;

        private KeyedReferencePool<string, HandleBase> pool;

        void Awake()
        {
            pool = GFGlobal.ReferencePool.CreatePool<string, HandleBase>("Resource", 1, -1, null, handle => handle.Release());
            defaultPackage = YooAssets.GetPackage(GameConfig.DefaultPackage);
        }

        public string LoadRawFileTextNoCache(string location)
        {
            var handle = defaultPackage.LoadRawFileSync(location);
            string text = handle.GetRawFileText();
            handle.Release();
            return text;
        }

        public UnityEngine.Object LoadAssetSyncByTagNoCache(string tag)
        {
            var assetInfos = defaultPackage.GetAssetInfos(tag);
            if (assetInfos.Length == 0)
            {
                Log.Error("[Resourace] not found asset with tag {0}", tag);
                return null;
            }

            string assetPath = assetInfos[0].AssetPath;
            var handle = defaultPackage.LoadAssetSync<InputActionAsset>(assetPath);
            return handle.AssetObject;
        }

        public IReadOnlyList<UnityEngine.Object> LoadAllAssetsSyncNoCache(string location)
        {
            AllAssetsHandle handle = defaultPackage.LoadAllAssetsSync<UnityEngine.Object>(location);
            return handle.AllAssetObjects;
        }

        private AssetHandle GetHandle<T>(string location, bool cache, bool isAsync) where T : UnityEngine.Object
        {
            AssetHandle handle = null;
            if (cache)
                handle = pool.Get(location) as AssetHandle;
            if (handle == null)
            {
                if (isAsync)
                    handle = defaultPackage.LoadAssetAsync<T>(location);
                else
                    handle = defaultPackage.LoadAssetSync<T>(location);
            }
            return handle;
        }

        public RequestHandle<T> LoadAssetAsync<T>(string location, Action<T, object> completed, object userData = null, bool cache = true) where T : UnityEngine.Object
        {
            AssetHandle handle = GetHandle<T>(location, cache, true);
            RequestHandle<T> requestHandle = new(handle, completed, userData, cache);
            if (cache)
                pool.Recycle(location, handle);
            return requestHandle;
        }

        public T LoadAssetSync<T>(string location, bool cache = true) where T : UnityEngine.Object
        {
            AssetHandle handle = GetHandle<T>(location, cache, false);
            if (cache)
                pool.Recycle(location, handle);
            return handle.GetAssetObject<T>();
        }

        public void LoadSceneSync(string location, LoadSceneMode sceneMode = LoadSceneMode.Single)
        {
            SceneHandle sceneHandle = defaultPackage.LoadSceneSync(location, sceneMode);
        }
    }
}