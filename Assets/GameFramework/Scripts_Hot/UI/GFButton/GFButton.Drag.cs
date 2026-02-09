using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    public partial class GFButton :
    IInitializePotentialDragHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            lastPointerClick = null;

            if (!IsActive() || !IsInteractable())
                return;

            this.eventData = eventData;
            if (scrollView)
            {
                eventFunctionType = ExecuteEvents.beginDragHandler.GetType();
                scrollView.OnBeginDrag(eventData);
            }

            onBeginDrag.Invoke(eventData);
            if (!isClickByDrag)
                eventData.eligibleForClick = false;
            if (onBeginDrag.Count != 0)
            {
                PassRaycast(ExecuteEvents.beginDragHandler);
                if (passedHandle != null && isNotifyNext)
                    passedDragHandle = passedHandle;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsActive() || !IsInteractable())
                return;

            this.eventData = eventData;
            if (scrollView)
            {
                eventFunctionType = ExecuteEvents.dragHandler.GetType();
                scrollView.OnDrag(eventData);
            }

            onDrag.Invoke(eventData);
            if (onDrag.Count != 0)
                PassRaycast(ExecuteEvents.dragHandler);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            this.eventData = eventData;
            onDrop.Invoke(eventData);
            if (onDrop.Count != 0)
                PassRaycast(ExecuteEvents.dropHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsActive() || !IsInteractable())
                return;

            this.eventData = eventData;
            if (scrollView)
            {
                eventFunctionType = ExecuteEvents.endDragHandler.GetType();
                scrollView.OnEndDrag(eventData);
            }

            onEndDrag.Invoke(eventData);
            if (onEndDrag.Count != 0)
                PassRaycast(ExecuteEvents.endDragHandler);
            passedDragHandle = null;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            this.eventData = eventData;

            if (scrollView)
            {
                eventFunctionType = ExecuteEvents.initializePotentialDrag.GetType();
                scrollView.OnInitializePotentialDrag(eventData);
            }

            onInitDrag.Invoke(eventData);
            if (onInitDrag.Count != 0)
                PassRaycast(ExecuteEvents.initializePotentialDrag);
        }

        /// <summary>
        /// 替换eventData中的数据，如果是拖动相关的事件则需要替换pointerDrag
        /// </summary>
        private void ChangeEventData<T>(GameObject handle, RaycastResult raycast) where T : IEventSystemHandler
        {
            Type t = typeof(T);
            if (t == typeof(IBeginDragHandler) || t == typeof(IDragHandler) || t == typeof(IEndDragHandler))
            {
                eventData.pointerDrag = handle;
            }
            eventData.pointerClick = handle;
            eventData.pointerEnter = handle;
            eventData.pointerPress = handle;
            eventData.pointerCurrentRaycast = raycast;
        }

        /// <summary>
        /// OnDisable的时候拖动不能中断，需要将eventData数据替换掉
        /// </summary>
        private void OnDisableCheckEvent()
        {
            if (passedDragHandle != null)
            {
                if (eventFunctionType == ExecuteEvents.beginDragHandler.GetType())
                {
                    ChangeEventData<IBeginDragHandler>(passedDragHandle, passedRaycast);
                    ExecuteEvents.Execute(passedDragHandle, eventData, ExecuteEvents.beginDragHandler);
                }
                else if (eventFunctionType == ExecuteEvents.dragHandler.GetType())
                {
                    ChangeEventData<IDragHandler>(passedDragHandle, passedRaycast);
                    ExecuteEvents.Execute(passedDragHandle, eventData, ExecuteEvents.dragHandler);
                }
                else if (eventFunctionType == ExecuteEvents.endDragHandler.GetType())
                {
                    ChangeEventData<IEndDragHandler>(passedDragHandle, passedRaycast);
                    ExecuteEvents.Execute(passedDragHandle, eventData, ExecuteEvents.endDragHandler);
                }
                passedDragHandle = null;
            }
            else if (scrollView != null && eventData != null && eventData.pointerDrag != null && gameObject == eventData.pointerDrag)
            {
                if (eventFunctionType == ExecuteEvents.beginDragHandler.GetType())
                {
                    ChangeEventData<IBeginDragHandler>(scrollView.gameObject, passedRaycast);
                    scrollView.OnBeginDrag(eventData);
                }
                else if (eventFunctionType == ExecuteEvents.dragHandler.GetType())
                {
                    ChangeEventData<IDragHandler>(scrollView.gameObject, passedRaycast);
                    scrollView.OnDrag(eventData);
                }
                else if (eventFunctionType == ExecuteEvents.endDragHandler.GetType())
                {
                    ChangeEventData<IEndDragHandler>(scrollView.gameObject, passedRaycast);
                    scrollView.OnEndDrag(eventData);
                }
                else if (eventFunctionType == ExecuteEvents.scrollHandler.GetType())
                {
                    ChangeEventData<IScrollHandler>(scrollView.gameObject, passedRaycast);
                    scrollView.OnScroll(eventData);
                }
                else if (eventFunctionType == ExecuteEvents.initializePotentialDrag.GetType())
                {
                    ChangeEventData<IInitializePotentialDragHandler>(scrollView.gameObject, passedRaycast);
                    scrollView.OnInitializePotentialDrag(eventData);
                }
            }
        }
    }
}