using System.Collections;
using UnityEngine;

namespace GameFramework.Hot
{
    [RequireComponent(typeof(RectTransform))]
    public class UICanvasSafeArea : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private ScreenOrientation _lastOrientation;
        private Rect _lastSafeArea;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            LazyUpdate();
        }

        private void OnEnable()
        {
            LazyUpdate();
        }

        private void SetSafeArea()
        {
            var safeArea = Screen.safeArea;
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    _rectTransform.offsetMin = new Vector2(safeArea.x, 0);
                    _rectTransform.offsetMax = new Vector2(-safeArea.x, 0);
                    break;
                case ScreenOrientation.LandscapeRight:
                    _rectTransform.offsetMax = new Vector2(-(Screen.width - safeArea.xMax), 0);
                    _rectTransform.offsetMin = new Vector2(Screen.width - safeArea.xMax, 0);
                    break;
            }
        }

        private void ForceUpdate()
        {
            _lastOrientation = Screen.orientation;
            _lastSafeArea = Screen.safeArea;
            SetSafeArea();
        }

        private void LazyUpdate()
        {
            StartCoroutine(LazyUpdateCoroutine());
        }

        private IEnumerator LazyUpdateCoroutine()
        {
            yield return new WaitForEndOfFrame();
            ForceUpdate();
        }

        private void Update()
        {
            if (Screen.orientation != _lastOrientation
#if UNITY_EDITOR
                || Screen.safeArea != _lastSafeArea
#endif
               )
            {
                ForceUpdate();
            }
        }

    }
}