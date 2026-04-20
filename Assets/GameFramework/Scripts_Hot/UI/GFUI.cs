using System;
using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    /// <summary>
    /// 一个面板由ControlType和guid组成
    /// 一个assetPath可以有多个controlType，一个controlType可以有多个guid
    /// 回收和利用根据controlType判断，Control和View一体，如果View没加载出来的话不回收
    /// </summary>
    public partial class GFUI : GFBaseModule
    {
        private bool defaultShowCursor = true;

        private Vector2 designResolution = new(1920, 1080);

        private RectTransform uiGroupRoot;
        private Dictionary<string, RectTransform> uiGroups = new();
        private KeyedReferencePool<Type, BaseControl> panelPool;
        private Dictionary<string, BaseControl> panels = new();
        private Dictionary<string, WaitOpenPanelInfo> waitOpenPanels = new();
        private HashSet<string> closingPanels = new();
        public Camera UICamera { get; private set; }

        void Awake()
        {
            // 回收隐藏，释放时面板和被拷贝的节点会直接销毁
            nodePool = GFGlobal.ReferencePool.CreatePool<int, BaseUINode>("UINode", 64, 60, PoolRecycleNode, PoolDestoryNode);
            panelPool = GFGlobal.ReferencePool.CreatePool<Type, BaseControl>("Panel", 64, 60, control => control.OnRecycle(), control => control.OnUIDestroy());

            // UI Form Instances
            var uiGroupGo = new GameObject("UI Group");
            uiGroupGo.transform.SetParent(transform);
            uiGroupGo.transform.localScale = Vector3.one;
            uiGroupGo.AddComponent<GraphicRaycaster>();
            uiGroupGo.AddComponent<CanvasGroup>();

            var canvasScaler = uiGroupGo.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.referenceResolution = designResolution;

            uiGroupRoot = uiGroupGo.GetComponent<RectTransform>();
            uiGroupGo.layer = LayerMask.NameToLayer("UI");

            foreach (var groupName in UIGroup.GROUPS)
                AddUIGruop(groupName);

            var canvas = uiGroupGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            // UI Camera
            GameObject cameraGo = new GameObject("UICamera");
            cameraGo.transform.SetParent(transform, true);
            cameraGo.transform.position = new Vector3(0, 1000, 0);

            UICamera = cameraGo.AddComponent<Camera>();
            UICamera.farClipPlane = 2000;
            UICamera.fieldOfView = 15;
            UICamera.depth = 1000;
            UICamera.cullingMask = LayerMask.GetMask("UI");
            UICamera.clearFlags = CameraClearFlags.Nothing; // 显示后面的主相机内容
            canvas.worldCamera = UICamera;

            var cameraData = UICamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            cameraData.SetRenderer(1); // UI界面专门的RendererData，可以用模糊效果
            cameraData.volumeLayerMask = LayerMask.GetMask("UI");
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            UpdateCursorVisible();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            UpdateCameraStack();
        }

        // 场景加载好后把UI相机挂到主相机的Stack里
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateCameraStack();
        }

        private void UpdateCameraStack()
        {
            Camera mainCamera = Camera.main;
            var uiData = UICamera.GetUniversalAdditionalCameraData();
            if (mainCamera == null)
            {
                uiData.renderType = CameraRenderType.Base;
                return;
            }

            var mainData = mainCamera.GetUniversalAdditionalCameraData();
            uiData.renderType = CameraRenderType.Overlay;
            if (!mainData.cameraStack.Contains(UICamera))
                mainData.cameraStack.Add(UICamera);
        }

        private void AddUIGruop(string name)
        {
            GameObject go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");

            go.transform.SetParent(uiGroupRoot.transform);
            go.transform.localScale = Vector3.one;

            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = uiGroups.Count;
            go.AddComponent<GraphicRaycaster>();
            go.AddComponent<CanvasGroup>();
            go.AddComponent<UICanvasSafeArea>();
            uiGroups.Add(name, go.GetComponent<RectTransform>());
        }

        private string GetNameWithGuid(Type controlType, int guid)
        {
            return $"{controlType.FullName}_{guid}";
        }

        public void OpenPanel<C>(int guid = 0, object userData = null) where C : BaseControl
        {
            string panelName = BaseControl.GetDefaultPanelName(typeof(C));
            bool hasData = GetPanelData(panelName, out string group, out string assetPath, out int _, out bool _);
            if (!hasData)
            {
                Log.Error("[UI] Panel not found: {0}", panelName);
                return;
            }

            // 界面预制件默认路径
            if (string.IsNullOrEmpty(assetPath))
                assetPath = string.Format(GFGlobal.Tables.TbGlobalSettingData.DefaultPanelPath, panelName);

            OpenPanel(typeof(C), group, assetPath, guid, userData);
        }

        public void OpenPanel<C>(string groupName, string assetPath, int guid = 0, object userData = null) where C : BaseControl
        {
            OpenPanel(typeof(C), groupName, assetPath, guid, userData);
        }

        public void OpenPanel(Type controlType, string groupName, string assetPath, int guid = 0, object userData = null)
        {
            Log.Info("Open UI Panel {0},{1},{2}", controlType.Name, groupName, assetPath);
            if (!uiGroups.ContainsKey(groupName))
            {
                Log.Error("[UI] UI Group not found: {0}", groupName);
                return;
            }

            string nameWithGuid = GetNameWithGuid(controlType, guid);
            if (panels.TryGetValue(nameWithGuid, out BaseControl control))
            {
                // 存一下，等面板关了再重新打开
                if (closingPanels.Contains(nameWithGuid))
                    waitOpenPanels[nameWithGuid] = new WaitOpenPanelInfo(controlType, groupName, assetPath, guid, userData);
                return;
            }

            control = panelPool.Get(controlType);
            bool needLoad = control == null;
            if (needLoad)
                control = Activator.CreateInstance(controlType) as BaseControl;
            control.InitControl(groupName, guid, assetPath);
            control.OnInit(userData);
            panels.Add(nameWithGuid, control);
            if (needLoad)
                GFGlobal.Resource.LoadAssetAsync<GameObject>(assetPath, OnPanelLoaded, (nameWithGuid, userData));
            else
                InitView(nameWithGuid, control.View.gameObject, userData);
        }

        private void OnPanelLoaded(GameObject prefab, object userData)
        {
            if (userData is ValueTuple<string, object> data && panels.ContainsKey(data.Item1))
                InitView(data.Item1, Instantiate(prefab), data.Item2);
        }

        private void InitView(string nameWithGuid, GameObject viewGo, object userData)
        {
            BaseControl control = panels[nameWithGuid];
            var group = uiGroups[control.UIGroup];
            viewGo.transform.SetParent(group, false);
            viewGo.SetActive(true);

            // 重置RectTransform属性，解决偏移问题
            if (viewGo.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }

            if (!viewGo.TryGetComponent<AbstractBaseView>(out var view))
            {
                Log.Error("{0}界面预制件没有挂载对应的View组件", viewGo.name);
                ClosePanel(control.GetType(), control.Guid, true);
                GameObject.Destroy(viewGo);
                return;
            }
            control.BindView(view);
            view.BindControl(control);
            view.OnInit(userData);

            UpdateCursorVisible();
            UpdatePanelPriority(nameWithGuid);

            GFGlobal.Event.Fire(this, OnPanelOpenEvent.Create(control.GetType()));
        }

        public void ClosePanel<C>(int guid = 0, bool immediate = false) where C : BaseControl
        {
            ClosePanel(typeof(C), guid, immediate);
        }

        public void ClosePanel(Type controlType, int guid = 0, bool immediate = false)
        {
            string nameWithGuid = GetNameWithGuid(controlType, guid);
            waitOpenPanels.Remove(nameWithGuid);
            if (!panels.TryGetValue(nameWithGuid, out BaseControl control))
                return;
            if (closingPanels.Contains(nameWithGuid))
                return; //正在关闭

            AbstractBaseView view = control.View;
            string assetPath = control.AssetPath;

            if (view != null)
            {
                float minEndTime = 0;
                if (!immediate)
                    minEndTime = view.PlayCloseAnimation();
                if (!Mathf.Approximately(minEndTime, 0))
                    GFGlobal.Timer.DelayRun(minEndTime, () => OnViewOutAnimEnd(nameWithGuid));
                else
                    OnViewOutAnimEnd(nameWithGuid);
            }
            else
            {
                // view没有创建，不缓存了
                panels.Remove(nameWithGuid);
                control.OnUIDestroy();
            }
        }

        private void OnViewOutAnimEnd(string nameWithGuid)
        {
            BaseControl control = panels[nameWithGuid];
            panels.Remove(nameWithGuid); //动画播放完毕才表示这个面板不存在了
            closingPanels.Remove(nameWithGuid);
            panelPool.Recycle(control.GetType(), control);

            if (waitOpenPanels.TryGetValue(nameWithGuid, out WaitOpenPanelInfo info))
            {
                OpenPanel(info.controlType, info.groupName, info.assetPath, info.guid, info.userData);
                waitOpenPanels.Remove(nameWithGuid);
            }

            UpdateCursorVisible();
        }

        public bool HasPanel<C>(int guid = 0) where C : BaseControl
        {
            string name = GetNameWithGuid(typeof(C), guid);
            return panels.ContainsKey(name);
        }

        private struct WaitOpenPanelInfo
        {
            public Type controlType;
            public string groupName;
            public string assetPath;
            public int guid;
            public object userData;
            public WaitOpenPanelInfo(Type controlType, string groupName, string assetPath, int guid, object userData)
            {
                this.controlType = controlType;
                this.groupName = groupName;
                this.assetPath = assetPath;
                this.guid = guid;
                this.userData = userData;
            }
        }

        public void UpdateCursorVisible()
        {
            foreach (var control in panels.Values)
            {
                if (control.ShowCursor)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    return;
                }
            }

            if (panels.Count > 0 || !defaultShowCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // 按照priority字段给面板排序
        private void UpdatePanelPriority(string nameWithGuid)
        {
            var control = panels[nameWithGuid];
            var view = control.View;
            GetPanelData(control.PanelName, out var _, out var _, out int priority, out var _);

            int targetIndex = -1;
            foreach (var pair in panels)
            {
                var control2 = pair.Value;
                if (pair.Key != nameWithGuid && control2.UIGroup == control.UIGroup && control2.View)
                {
                    GetPanelData(control2.PanelName, out var _, out var _, out int p, out var _);
                    if (p > priority)
                    {
                        if (targetIndex == -1)
                            targetIndex = control2.View.transform.GetSiblingIndex();
                        else
                            targetIndex = Mathf.Min(control2.View.transform.GetSiblingIndex(), targetIndex);
                    }
                }
            }
            if (targetIndex != -1)
                view.transform.SetSiblingIndex(targetIndex);
        }

        private List<BaseControl> tempControls = new();

        // 开始加载时把配置了LoadingClose的面板都关闭
        public void OnStartLoading()
        {
            tempControls.AddRange(panels.Values);
            for (int i = 0; i < tempControls.Count; i++)
            {
                var control = tempControls[i];
                if (!(control.UIGroup == UIGroup.SCENE || control.UIGroup == UIGroup.FILTER || control.UIGroup == UIGroup.MAIN))
                    continue;

                GetPanelData(control.PanelName, out var _, out var _, out var _, out bool loadingClose);
                if (loadingClose)
                    control.Close(true);
            }
            tempControls.Clear();
        }


        private bool GetPanelData(string panelName, out string group, out string assetPath, out int priority, out bool loadingClose)
        {
            group = UIGroup.MAIN;
            assetPath = "";
            priority = 0;
            loadingClose = true;
#if USE_LUBAN
            var panelData = GFGlobal.Tables.TbPanelData.GetOrDefault(panelName);
            if (panelData == null)
                return false;

            group = panelData.Group;
            assetPath = panelData.AssetPath;
            priority = panelData.Priority;
            loadingClose = panelData.LoadingClose;
            return true;
#else
            // 不使用luban需要自定义这一部分
            Log.Error("不使用luban需要自定义面板数据这一部分");
            return false;
#endif
        }
    }
}