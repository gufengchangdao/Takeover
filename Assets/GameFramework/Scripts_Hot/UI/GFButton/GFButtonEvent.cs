using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    /// <summary>
    /// 按钮事件，主要是用来判断当前按钮是否有无正在监听的事件
    /// 注意这个计数并不是特别准，有可能偏大，只是用来优化判断的
    /// </summary>
    public class GFButtonEvent : UnityEvent<BaseEventData>
    {
        /// <summary>
        /// 按钮事件监听计数，注意这个计数并不是特别准，有可能偏大，只是用来优化判断的
        /// </summary>
        public int Count { get; private set; }

        public void AddEventListener(UnityAction<BaseEventData> call)
        {
            AddListener(call);
            Count++;
        }

        public void RemoveEventListener(UnityAction<BaseEventData> call)
        {
            RemoveListener(call);
            Count--;
        }

        public void Clear()
        {
            RemoveAllListeners();
            Count = 0;
        }
    }
}