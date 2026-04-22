using System;
using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine;

namespace GameFramework.Hot
{
    public abstract partial class AbstractBaseView : MonoBehaviour, IUILifecycle
    {
        private HashSet<int> cloneSourceIds = new();

        public event Action OnPanelClose;

        public abstract void BindControl(BaseControl control);

        public virtual void OnInit(object userData)
        {
            PlayInAnimation();
            foreach (var node in GetComponentsInChildren<BaseUINode>(true))
            {
                node.OnInit();
            }
        }

        public virtual void OnRecycle()
        {
            // 告知其他组件，移除一些监听
            OnPanelClose.InvokeSafe();
            OnPanelClose = null;

            // 回收拷贝出来的节点
            foreach (var node in GetComponentsInChildren<BaseUINode>(true))
            {
                if (node.SourceId != 0) //等于0表示是包装后的节点，不是克隆的，不用动
                    GFGlobal.UI.RecycleNode(node);
                else
                    node.OnRecycle();
            }

            // 停止定时器
            CancelAllTimers();
        }

        public virtual void OnUIDestroy()
        {
            // 销毁缓存节点
            foreach (var sourceId in cloneSourceIds)
                GFGlobal.UI.DestroyCacheNode(sourceId);
            cloneSourceIds.Clear();
        }

        #region 动画
        public const string PanelInAnim = "PanelIn";
        public const string PanelOutAnim = "PanelOut";

        public float PlayInAnimation()
        {
            if (TryGetComponent<Animator>(out var animator))
                return animator.GFPlayUIAnimation(PanelInAnim, true);
            return 0;
        }

        public float PlayCloseAnimation()
        {
            if (TryGetComponent<Animator>(out var animator))
                return animator.GFPlayUIAnimation(PanelOutAnim, true);
            return 0;
        }
        #endregion

        /// <summary>
        /// 拷贝一个节点
        /// </summary>
        public T CloneNode<T>(T node) where T : BaseUINode, new()
        {
            int id = node.SourceId;
            if (id == 0)
                id = node.gameObject.GetInstanceID();

            T newNode = GFGlobal.UI.GetCacheNode<T>(id);
            if (newNode == null)
            {
                var newGo = GameObject.Instantiate(node.gameObject, node.transform.parent);
                newGo.SetActive(true);
                newNode = newGo.GFGetOrAddComponent<T>();
                newNode.SourceId = id;
            }
            newNode.OnInit();
            cloneSourceIds.Add(id);
            return newNode;
        }

        public abstract void Close(bool immediate = false);
    }

    public class BaseView<C> : AbstractBaseView where C : BaseControl
    {
        public C Control { get; private set; }

        public int SerialId => Control != null ? Control.Guid : 0;
        public string AssetPath => Control != null ? Control.AssetPath : string.Empty;

        public override sealed void BindControl(BaseControl control)
        {
            if (control is C)
                Control = (C)control;
            else
                Log.Error("[UI] control类型不对" + control.GetType() + "     " + typeof(C));
        }

        public override void Close(bool immediate = false)
        {
            Control.Close(immediate);
        }
    }
}