using System.Collections;
using UnityEngine;
using YooAsset;

namespace GameFramework.AOT
{
    public class PatchOperation : GameAsyncOperation
    {
        private Launch launch;
        private BasePatchWindow patchWindow;

        private readonly string packageName;
        private string defaultPackageVersion;
        private EPlayMode playMode;
        private string packageVersion;
        private readonly string mainUrl;
        private readonly string fallbackUrl;
        private bool useOldVersion;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="defaultPackageVersion">首包资源版本</param>
        /// <param name="playMode">游玩模式</param>
        /// <param name="mainUrl">默认资源请求地址</param>
        /// <param name="fallbackUrl">缺省资源请求地址</param>
        public PatchOperation(Launch launch,
            string packageName,
            string defaultPackageVersion,
            EPlayMode playMode,
            string mainUrl = null,
            string fallbackUrl = null)
        {
            this.launch = launch;
            this.packageName = packageName;
            this.defaultPackageVersion = defaultPackageVersion;
            this.playMode = playMode;
            this.mainUrl = mainUrl;
            this.fallbackUrl = fallbackUrl ?? mainUrl;
        }

        protected override void OnAbort()
        {
        }

        protected override void OnStart()
        {
            patchWindow = GameObject.FindAnyObjectByType<BasePatchWindow>();
            launch.StartCoroutine(InitPackage());
        }

        private void SetDone()
        {
            Status = EOperationStatus.Succeed;
            if (patchWindow)
            {
                GameObject.Destroy(patchWindow.gameObject);
                patchWindow = null;
            }
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnWaitForAsyncComplete()
        {
            base.OnWaitForAsyncComplete();
        }

        private IEnumerator InitPackage()
        {
            Log.Info("[YooAsset] 开始初始化资源包");
            // 创建资源包裹类
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
                package = YooAssets.CreatePackage(packageName);

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = buildResult.PackageRootDirectory;
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (playMode == EPlayMode.HostPlayMode)
            {
                var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                buildinFileSystemParams.AddParameter(FileSystemParametersDefine.COPY_BUILDIN_PACKAGE_MANIFEST, true); //初始化的时候拷贝内置清单到沙盒目录

                // 注意：设置参数INSTALL_CLEAR_MODE，可以解决覆盖安装的时候将拷贝的内置清单文件清理的问题。
                var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(new RemoteServices(mainUrl, fallbackUrl));
                cacheFileSystemParams.AddParameter(FileSystemParametersDefine.INSTALL_CLEAR_MODE, EOverwriteInstallClearMode.None);

                var createParameters = new HostPlayModeParameters();
                createParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                createParameters.CacheFileSystemParameters = cacheFileSystemParams;
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;

            // 如果初始化失败弹出提示界面
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                Log.Info("[YooAsset] 资源包初始化成功");
                yield return UpdatePackageVersion();
            }
            else
            {
                var errMsg = $"[YooAsset] 资源包初始化失败: {initializationOperation.Error}";
                Log.Error(errMsg);
                yield return patchWindow.ShowErrorMessage(errMsg);
                yield return InitPackage();
            }
        }

        private IEnumerator UpdatePackageVersion()
        {
            Log.Info("[YooAsset] 开始更新资源版本");

            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                packageVersion = operation.PackageVersion;
                Log.Info($"Request package version : {packageVersion}");
                yield return UpdateManifest();
            }
            else
            {
                var errMsg = $"[YooAsset] 更新资源版本失败: {operation.Error}";
                Log.Error(errMsg);

                // yield return patchWindow.ShowErrorMessage("网络连接失败，是否使用本地资源启动游戏？", "是", "否");
                yield return patchWindow.ShowErrorMessage("Try?", "Yes", "No"); //重试或者使用旧的版本

                if (patchWindow.GetOption() == 0)
                {
                    yield return UpdatePackageVersion();
                }
                else
                {
                    useOldVersion = true;
                    packageVersion = PlayerPrefs.GetString("GAME_VERSION", defaultPackageVersion); // 获取上次成功记录的版本
                    Log.Debug("使用旧版本加载资源: {0}", packageVersion);
                    yield return UpdateManifest();
                }
            }
        }

        private IEnumerator UpdateManifest()
        {
            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                yield return CreateDownloader();
            }
            else
            {
                var errMsg = $"[YooAsset] 资源清单更新失败: {operation.Error}";
                Log.Error(errMsg);
                yield return patchWindow.ShowErrorMessage("update fail!", "Retry");
                yield return UpdateManifest();
            }
        }

        private IEnumerator CreateDownloader()
        {
            var package = YooAssets.GetPackage(packageName);
            var downloader = package.CreateResourceDownloader(10, 3);

            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("Not found any download files !");
                SetDone();
                yield break;
            }

            if (useOldVersion)
            {
                // 如果是使用旧版本的话，在正常开始游戏之前，还需要验证本地清单内容的完整性。
                if (downloader.TotalDownloadCount > 0)
                {
                    Log.Debug("资源内容本地并不完整，开始使用首包资源！");
                    packageVersion = defaultPackageVersion;
                    yield return UpdateManifest();
                    yield break;
                }
            }

            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            float sizeMB = downloader.TotalDownloadBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            Log.Info($"Found update patch files, Total count {downloader.TotalDownloadCount} Total szie {totalSizeMB}MB");

            Log.Info("开始下载资源！");
            downloader.DownloadErrorCallback = OnDownloadError;
            downloader.DownloadUpdateCallback = OnDownloadUpdate;
            downloader.BeginDownload();
            yield return downloader;

            if (downloader.Status == EOperationStatus.Succeed)
            {
                Log.Info("资源文件下载完毕！");
                PlayerPrefs.SetString("GAME_VERSION", packageVersion); // 下载完成后再保存资源版本
                // 清理未使用的缓存文件
                yield return package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
                SetDone();
            }
        }

        private void OnDownloadError(DownloadErrorData errorData)
        {
            launch.StartCoroutine(TryCreateDownloader(errorData));
        }

        /// <summary>
        /// 重试下载
        /// </summary>
        private IEnumerator TryCreateDownloader(DownloadErrorData errorData)
        {
            yield return patchWindow.ShowErrorMessage($"Failed to download file : {errorData.FileName}");
            yield return CreateDownloader();
        }

        private void OnDownloadUpdate(DownloadUpdateData updateData)
        {
            patchWindow.UpdateProgress(updateData);
        }

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private string mainUrl;
            private string fallbackUrl;

            public RemoteServices(string mainUrl, string fallbackUrl)
            {
                this.mainUrl = mainUrl;
                this.fallbackUrl = fallbackUrl;
            }

            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{mainUrl}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{fallbackUrl}/{fileName}";
                // return $"http://127.0.0.1/CDN/PC/v1.0/{fileName}";
            }
        }
    }
}