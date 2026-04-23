using System;
using YooAsset;

namespace GameFramework.Hot
{
    public interface IRequestHandle
    {
        void Cancel();
    }

    /// <summary>
    /// 请求回调，主要提供一个移除回调执行的方法，一般用于请求者生命周期结束时
    /// </summary>
    public class RequestHandle<T> : IRequestHandle where T : UnityEngine.Object
    {
        private AssetHandle handle;
        private bool cache;
        private Action<T, object> completed;
        private object userData;

        public RequestHandle(AssetHandle handle, Action<T, object> completed, object userData, bool cache)
        {
            this.handle = handle;
            this.completed = completed;
            this.userData = userData;
            this.cache = cache;

            handle.Completed += OnAssetLoaded;
        }

        private void OnAssetLoaded(AssetHandle assetHandle)
        {
            completed.InvokeSafe(assetHandle.GetAssetObject<T>(), userData);
            Cancel();
        }

        public void Cancel()
        {
            if (handle == null) return;
            handle.Completed -= OnAssetLoaded;
            if (!cache)
                handle.Release();
            handle = null;
            completed = null;
            userData = null;
        }
    }
}