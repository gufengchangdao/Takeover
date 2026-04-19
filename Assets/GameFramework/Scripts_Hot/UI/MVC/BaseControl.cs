using System;
using GameFramework.AOT;
using UnityEngine;

namespace GameFramework.Hot
{
    public abstract class BaseControl : IUILifecycle
    {
        // 面板名字，默认去掉Control就是面板名
        public virtual string PanelName
        {
            get
            {
                return GetDefaultPanelName(GetType());
            }
        }

        public string UIGroup { get; private set; }
        public int Guid { get; private set; }
        public string AssetPath { get; private set; }
        public AbstractBaseView View { get; private set; }

        private bool m_showCursor = true;
        /// <summary>
        /// 是否显示鼠标
        /// </summary>
        public bool ShowCursor
        {
            get
            {
                return m_showCursor;
            }
            set
            {
                m_showCursor = value;
                GFGlobal.UI.UpdateCursorVisible();
            }
        }

        /// <summary>
        /// 层级的优先级，有些界面加载的慢但又要求在其他界面前面，通过指定优先级解决，值越小越在上面
        /// </summary>
        public int LayerPriority { get; set; }

        public static string GetDefaultPanelName(Type controlType)
        {
            string suffix = "Control";
            string className = controlType.Name;
            if (className.EndsWith(suffix))
                return className.Substring(0, className.Length - suffix.Length);
            return className;
        }

        public void InitControl(string uiGroup, int guid, string assetPath)
        {
            UIGroup = uiGroup;
            Guid = guid;
            AssetPath = assetPath;
        }

        public void BindView(AbstractBaseView view)
        {
            View = view;
        }

        public virtual void OnInit(object userData)
        {
        }

        public virtual void OnRecycle()
        {
            if (View)
            {
                View.gameObject.SetActive(false);
                View.OnRecycle();
            }
            else
            {
                Log.Error("View is null,Control:{0} AssetPath: {1}", GetType().Name, AssetPath);
            }
            UIGroup = null;
            Guid = 0;
        }

        public virtual void OnUIDestroy()
        {
            if (View)
            {
                View.OnUIDestroy();
                GameObject.Destroy(View.gameObject);
            }
            View = null;
        }

        public void Close(bool immediate = false)
        {
            GFGlobal.UI.ClosePanel(GetType(), Guid, immediate);
        }
    }
}