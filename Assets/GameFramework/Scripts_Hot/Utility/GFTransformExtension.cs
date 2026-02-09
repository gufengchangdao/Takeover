using UnityEngine;

public static class GFTransformExtension
{
    public static T GFFindComponent<T>(this Transform transform, string path = null) where T : Component
    {
        Transform node = (path == null || path == "" || path == ".") ? transform : transform.Find(path);
        if (node == null)
            return null;
        node.TryGetComponent(out T c);
        return c;
    }

    /// <summary>
    /// 将世界坐标转换为某个UI对象应该设置的局部坐标
    /// </summary>
    /// <param name="transform">要设置坐标的UI对象</param>
    /// <param name="worldPoint">UI对象需要在的位置，世界坐标</param>
    /// <param name="mainCamera">相机</param>
    /// <returns>UI对象对应的位置，可以直接设置为UI对象的局部坐标</returns>
    public static Vector2 WorldToUILocalPosition(this Transform transform, Vector3 worldPoint, Camera canvasCamera = null, Camera mainCamera = null)
    {
        // 获取目标RectTransform
        var targetRectTransform = transform.parent as RectTransform;
        if (targetRectTransform == null)
        {
            Debug.LogWarning("Transform must have a RectTransform component");
            return Vector2.zero;
        }

        // 获取Canvas的Camera
        if (canvasCamera == null)
        {
            var canvas = transform.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Transform must be under a Canvas");
                return Vector2.zero;
            }
            canvasCamera = canvas.worldCamera;
        }

        // 将世界坐标转换为屏幕坐标
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("Camera.main is null");
                return Vector2.zero;
            }
        }
        Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldPoint);

        // 转换屏幕坐标到UI坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTransform, screenPoint, canvasCamera, out Vector2 uiPosition);
        return uiPosition;
    }
}