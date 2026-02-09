using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    public partial class GFButton :
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsActive() || !IsInteractable())
                return;
            this.eventData = eventData;

            if (onDoubleClick.Count <= 0)
            {
                // 没双击事件可以立即处理单击
                SingleClick();
                return;
            }

            // 判断双击
            if (lastPointerClick != null // 不是第一次点击
                && lastPointerClick == eventData.pointerClick // 两次点击对象一样
                && Time.realtimeSinceStartup - lastClickTimes <= doubleClickTime) // 两次点击间隔小于指定时间
            {
                lastPointerClick = null;
                onDoubleClick.Invoke(eventData);
            }
            else
            {
                lastPointerClick = this.eventData.pointerClick;
                lastClickTimes = Time.realtimeSinceStartup;
            }
        }

        private void SingleClick()
        {
            lastPointerClick = null;
            onClick.Invoke(eventData);
            if (onClick.Count != 0)
                PassRaycast(ExecuteEvents.pointerClickHandler);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
                return; //跟Selectable保持一致，父类判断了我也判断
            if (!IsActive() || !IsInteractable())
                return;
            this.eventData = eventData;

            isPointerDown = true;
            lastPointerDownTimes = Time.realtimeSinceStartup;
            lastPointerContinueDownTimes = lastPointerDownTimes;

            onPointerDown.Invoke(eventData);
            if (onPointerDown.Count != 0)
                PassRaycast(ExecuteEvents.pointerDownHandler);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (!IsActive() || !IsInteractable())
                return;
            this.eventData = eventData;

            onEnter.Invoke(eventData);
            if (onEnter.Count != 0)
                PassRaycast(ExecuteEvents.pointerEnterHandler);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (!IsActive() || !IsInteractable())
                return;
            this.eventData = eventData;

            onExit.Invoke(eventData);
            if (onExit.Count != 0)
                PassRaycast(ExecuteEvents.pointerExitHandler);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isPointerDown = false;

            if (!IsActive() || !IsInteractable())
                return;
            this.eventData = eventData;

            onPointerUp.Invoke(eventData);
            if (onPointerUp.Count != 0)
                PassRaycast(ExecuteEvents.pointerUpHandler);
        }


        /// <summary>
        /// 判断监听双击事件时 是否触发单击事件
        /// </summary>
        private void CheckSingleClick()
        {
            if (lastPointerClick != null && Time.realtimeSinceStartup - lastClickTimes > doubleClickTime)
                SingleClick();
        }

        /// <summary>
        /// 判断长按单次触发与长按多次触发事件
        /// </summary>
        private void CheckLongSingleClick()
        {
            if (!isPointerDown)
                return;

            if (onLongPointerDown.Count > 0
                && lastPointerDownTimes != 0
                && Time.realtimeSinceStartup - lastPointerDownTimes >= longPointerDownTime)
            {
                lastPointerDownTimes = 0;
                onLongPointerDown.Invoke(eventData);
            }

            if (onLongPointerContinueDown.Count > 0
                && lastPointerContinueDownTimes != 0
                && Time.realtimeSinceStartup - lastPointerContinueDownTimes >= longPointerDownContinueTime)
            {
                lastPointerContinueDownTimes = Time.realtimeSinceStartup;
                onLongPointerContinueDown.Invoke(eventData);
            }
        }
    }
}