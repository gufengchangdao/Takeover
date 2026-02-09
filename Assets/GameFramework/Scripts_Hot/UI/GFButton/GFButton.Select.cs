using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    public partial class GFButton :
    IUpdateSelectedHandler,
    ISelectHandler,
    IDeselectHandler,
    IMoveHandler
    {
        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            onUpdateSelected.Invoke(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            onSelect.Invoke(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            onDeSelect.Invoke(eventData);
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            onMove.Invoke(eventData);
        }
    }
}