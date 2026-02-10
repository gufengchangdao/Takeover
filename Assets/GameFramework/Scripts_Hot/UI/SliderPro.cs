using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    public class SliderPro : Slider, IEndDragHandler
    {
        /// <summary>
        /// 值改变回调，松开时才会触发
        /// </summary>

        private float _clickDownValue;
        public SliderEvent onDelayValueChanged;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _clickDownValue = value;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!Mathf.Approximately(_clickDownValue, value))
                onDelayValueChanged.Invoke(value);
        }
    }
}