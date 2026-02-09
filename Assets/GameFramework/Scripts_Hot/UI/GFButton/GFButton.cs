using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    /// <summary>
    /// 左键点击
    /// </summary>
    [DisallowMultipleComponent]
    public partial class GFButton : Selectable
    {
        /// <summary>
        /// 是否传递事件到下一层
        /// </summary>
        [Tooltip("是否传递事件到下一层")]
        public bool isNotifyNext = false;

        /// <summary>
        /// 拖动是否触发点击
        /// </summary>
        [Tooltip("拖动是否触发点击")]
        public bool isClickByDrag = false;

        /// <summary>
        /// 双击触发间隔
        /// </summary>
        [Tooltip("双击触发间隔")]
        public float doubleClickTime = 0.2f;

        /// <summary>
        /// 长按触发时间
        /// </summary>
        [Tooltip("长按触发时间")]
        public float longPointerDownTime = 0.5f;
        /// <summary>
        /// 连续点击触发间隔
        /// </summary>
        [Tooltip("连续点击触发间隔")]
        public float longPointerDownContinueTime = 0.15f;

        /// <summary>
        /// 记录每次点击数据
        /// </summary>
        private PointerEventData eventData;

        public GFButtonEvent onClick = new GFButtonEvent();
        public GFButtonEvent onDoubleClick = new GFButtonEvent();
        public GFButtonEvent onLongPointerDown = new GFButtonEvent();
        public GFButtonEvent onLongPointerContinueDown = new GFButtonEvent();
        public GFButtonEvent onPointerDown = new GFButtonEvent();
        public GFButtonEvent onEnter = new GFButtonEvent();
        public GFButtonEvent onExit = new GFButtonEvent();
        public GFButtonEvent onPointerUp = new GFButtonEvent();
        public GFButtonEvent onBeginDrag = new GFButtonEvent();
        public GFButtonEvent onDrag = new GFButtonEvent();
        public GFButtonEvent onEndDrag = new GFButtonEvent();
        public GFButtonEvent onDrop = new GFButtonEvent();
        public GFButtonEvent onInitDrag = new GFButtonEvent();
        public GFButtonEvent onScroll = new GFButtonEvent();
        public GFButtonEvent onSelect = new GFButtonEvent();
        public GFButtonEvent onDeSelect = new GFButtonEvent();
        public GFButtonEvent onUpdateSelected = new GFButtonEvent();
        public GFButtonEvent onMove = new GFButtonEvent();
        public GFButtonEvent onSubmit = new GFButtonEvent();
        public GFButtonEvent onCancel = new GFButtonEvent();

        private ScrollRect scrollView;
        private bool isPointerDown = false;
        private float lastPointerDownTimes;
        private float lastPointerContinueDownTimes;
        private float lastClickTimes;
        /// <summary>
        /// 是否有事件通知到下一层
        /// </summary>
        public bool HasNotifiedNext { get; private set; } = false;

        /// <summary>
        /// 标记是否穿透过
        /// </summary>
        private bool hasPassedEvent = false;

        /// <summary>
        /// 记录上一次点击的对象，用于双击判断
        /// </summary>
        private GameObject lastPointerClick;

        /// <summary>
        /// 穿透的handle
        /// </summary>
        private GameObject passedHandle;
        /// <summary>
        /// 记录穿透时的event type
        /// </summary>
        private Type eventFunctionType;

        /// <summary>
        /// 穿透的拖动事件handle
        /// </summary>
        private GameObject passedDragHandle;

        /// <summary>
        /// 穿透时的射线
        /// </summary>
        private RaycastResult passedRaycast;


        protected override void Awake()
        {
            base.Awake();
            FindParentScrollRect();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            eventFunctionType = null;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            FindParentScrollRect();
        }
#endif

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            FindParentScrollRect();
        }

        public void FindParentScrollRect()
        {
            scrollView = null;
            var parent = transform.parent;
            while (parent != null)
            {
                if (parent.TryGetComponent<ScrollRect>(out var parentScrollView))
                {
                    scrollView = parentScrollView;
                    break;
                }
                else
                {
                    parent = parent.parent;
                }
            }
        }

        protected override void OnDisable()
        {
            OnDisableCheckEvent();
            passedRaycast.Clear();
            eventFunctionType = null;
            hasPassedEvent = false;
            lastPointerClick = null;
            isPointerDown = false;
            passedHandle = null;
            base.OnDisable();
        }

        void Update()
        {
            if (!IsActive() || !IsInteractable())
                return;

            CheckSingleClick();
            CheckLongSingleClick();
        }

        private void PassRaycast<T>(ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            passedHandle = null;
            eventFunctionType = function.GetType();

            if (!isNotifyNext)
                return;

            PassEvent(function);
        }


        public void PassEvent<T>(ExecuteEvents.EventFunction<T> function)
                    where T : IEventSystemHandler
        {
            HasNotifiedNext = false;
            if (hasPassedEvent)
                return;

            hasPassedEvent = true;

            // 触发拖动事件则优先找BeginDrag抓到的handle
            if (passedDragHandle != null)
            {
                GameObject handle = null;
                if (eventFunctionType == ExecuteEvents.dragHandler.GetType())
                {
                    handle = ExecuteEvents.GetEventHandler<IDragHandler>(passedDragHandle);
                }
                else if (eventFunctionType == ExecuteEvents.endDragHandler.GetType())
                {
                    handle = ExecuteEvents.GetEventHandler<IEndDragHandler>(passedDragHandle);
                }

                if (handle != null)
                {
                    ExecuteEvents.Execute(handle, eventData, function);
                    HasNotifiedNext = true;
                    hasPassedEvent = false;
                    return;
                }
            }

            // 非拖动的事件则重新打射线去检测
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            GameObject current = ExecuteEvents.GetEventHandler<T>(eventData.pointerCurrentRaycast.gameObject);
            for (int i = 0; i < results.Count; i++)
            {
                GameObject handle = ExecuteEvents.GetEventHandler<T>(results[i].gameObject);
                if (handle == null)
                    break;

                if (handle.GetComponent<ScrollRect>())
                    break;

                if (current == handle)
                    continue; // 不能是自己

                if (handle.TryGetComponent<GFButton>(out var button) && button.hasPassedEvent)
                    continue; // 目标按钮已处理过穿透 

                ExecuteEvents.Execute(handle, eventData, function);
                HasNotifiedNext = true;
                passedHandle = handle;
                passedRaycast = results[i];
                break;
            }
            hasPassedEvent = false;
        }

        public void RemoveAll()
        {
            onEnter.Clear();
            onExit.Clear();
            onClick.Clear();
            onInitDrag.Clear();
            onBeginDrag.Clear();
            onDrag.Clear();
            onEndDrag.Clear();
            onPointerUp.Clear();
            onPointerDown.Clear();
            onDoubleClick.Clear();
            onLongPointerDown.Clear();
            onLongPointerContinueDown.Clear();
            onDrop.Clear();
            onUpdateSelected.Clear();
            onSelect.Clear();
            onDeSelect.Clear();
            onMove.Clear();
            onSubmit.Clear();
            onCancel.Clear();
            onScroll.Clear();
        }
    }
}