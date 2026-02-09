using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.Hot
{
    public partial class GFButton :
        ISubmitHandler,
        ICancelHandler
    {
        public void OnCancel(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            onCancel.Invoke(eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            onSubmit.Invoke(eventData);
            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}