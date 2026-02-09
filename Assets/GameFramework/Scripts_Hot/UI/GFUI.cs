using System;
using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

            UpdateCursorVisible();
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

        public void OpenPanel<C>(string groupName, string assetPath, int guid = 0, object userData = null) where C : BaseControl
        {
            OpenPanel(typeof(C), groupName, assetPath, guid, userData);
        }

        public void OpenPanel(Type controlType, string groupName, string assetPath, int guid = 0, object userData = null)
        {
            if (!uiGroups.ContainsKey(groupName))
            {
                Log.Error("[UI] UI Group not found: {0}", groupName);
                return;
            }

            string name = GetNameWithGuid(controlType, guid);
            if (panels.TryGetValue(name, out BaseControl control))
            {
                if (!control.IsClosing)
                    return;

                // 存一下，等面板关了再重新打开
                waitOpenPanels.Add(name, new WaitOpenPanelInfo(controlType, groupName, assetPath, guid, userData));
            }

            control = panelPool.Get(controlType);
            bool needLoad = control == null;
            if (needLoad)
                control = Activator.CreateInstance(controlType) as BaseControl;
            control.InitControl(groupName, guid, assetPath);
            control.OnInit(userData);
            panels.Add(name, control);
            if (needLoad)
                GFGlobal.Resource.LoadAssetAsync<GameObject>(assetPath, OnPanelLoaded, name);
            else
                InitView(name, control.View.gameObject);
        }

        private void OnPanelLoaded(GameObject prefab, object userData)
        {
            string name = userData as string;
            if (name != null && panels.ContainsKey(name)) //有可能面板已经关闭了
                InitView(name, Instantiate(prefab));
        }

        private void InitView(string name, GameObject viewGo)
        {
            BaseControl control = panels[name];
            var group = uiGroups[control.UIGroup];
            viewGo.transform.SetParent(group, false);
            viewGo.SetActive(true);

            // 重置RectTransform属性，解决偏移问题
            var rectTransform = viewGo.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }

            var view = viewGo.GetComponent<AbstractBaseView>();
            control.BindView(view);
            view.BindControl(control);
            view.OnInit(null);

            UpdateCursorVisible();

            GFGlobal.Event.Fire(this, OnPanelOpenEvent.Create(control.GetType()));
        }

        public void ClosePanel<C>(int guid = 0, bool immediate = false) where C : BaseControl
        {
            ClosePanel(typeof(C), guid, immediate);
        }

        public void ClosePanel(Type controlType, int guid = 0, bool immediate = false)
        {
            string name = GetNameWithGuid(controlType, guid);
            waitOpenPanels.Remove(name);
            if (!panels.TryGetValue(name, out BaseControl control))
                return;
            if (control.IsClosing)
                return;

            AbstractBaseView view = control.View;
            string assetPath = control.AssetPath;

            if (view != null)
            {
                float minEndTime = 0;
                if (!immediate)
                    minEndTime = control.PlayCloseAnimation();
                if (!Mathf.Approximately(minEndTime, 0))
                    GFGlobal.Timer.DelayRun(minEndTime, () => OnViewOutAnimEnd(name));
                else
                    OnViewOutAnimEnd(name);
            }
            else
            {
                // view没有创建，不缓存了
                panels.Remove(name);
                control.OnUIDestroy();
            }
        }

        private void OnViewOutAnimEnd(string name)
        {
            BaseControl control = panels[name];
            panels.Remove(name); //动画播放完毕才表示这个面板不存在了
            panelPool.Recycle(control.GetType(), control);

            if (waitOpenPanels.TryGetValue(name, out WaitOpenPanelInfo info))
            {
                OpenPanel(info.controlType, info.groupName, info.assetPath, info.guid, info.userData);
                waitOpenPanels.Remove(name);
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
    }
}