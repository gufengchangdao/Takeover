using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    public partial class GFButton :
    IScrollHandler
    {
        public void OnScroll(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            this.eventData = eventData;
            if (scrollView)
            {
                eventFunctionType = ExecuteEvents.scrollHandler.GetType();
                scrollView.OnScroll(eventData);
            }

            onScroll.Invoke(eventData);
            if (onScroll.Count != 0)
                PassRaycast(ExecuteEvents.scrollHandler);
        }
    }
}