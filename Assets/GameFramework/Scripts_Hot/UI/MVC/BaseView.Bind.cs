using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    //因为面板是回收再使用的，面板关闭时需要清理一些回调，这里封装一些方法在面板关闭时自动清理回调
    public partial class AbstractBaseView
    {
        private void RegisterOnClose(Action cleanup)
        {
            OnPanelClose += cleanup;
        }

        /// <summary>
        /// 按钮点击
        /// </summary>
        public GFButton BtnOnClick(GFButton btn, UnityAction<BaseEventData> call)
        {
            btn.onClick.AddEventListener(call);
            RegisterOnClose(() =>
            {
                if (btn) //考虑按钮可能是动态生成又中间销毁的
                    btn.onClick.RemoveEventListener(call);
            });
            return btn;
        }

        /// <summary>
        /// 滚动条值改变
        /// </summary>
        public GFSlider SliderOnValueChanged(GFSlider slider, UnityAction<float> call)
        {
            slider.onValueChanged.AddListener(call);
            RegisterOnClose(() =>
            {
                if (slider)
                    slider.onValueChanged.RemoveListener(call);
            });
            return slider;
        }
    }
}