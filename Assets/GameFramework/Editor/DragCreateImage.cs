using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 从Project窗口拖动图片到Canvas下任意层级，自动生成对应Image
/// </summary>
public class DragCreateImage
{
    private static bool isTrigger = false;

    [InitializeOnLoadMethod]
    private static void Init()
    {
        EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGui;
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGui;
        EditorApplication.hierarchyChanged -= HierarchyWindowChanged;
        EditorApplication.hierarchyChanged += HierarchyWindowChanged;
    }

    private static void ProjectWindowItemOnGui(string guid, Rect selectionRect)
    {
        if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited)
            isTrigger = true;
    }

    private static void HierarchyWindowChanged()
    {
        if (!isTrigger)
            return;

        // 此时Unity会默认创建Sprite并定位到该GameObject上
        GameObject go = Selection.activeGameObject;
        if (go == null)
            return;

        if (!go.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            return;

        if (go.GetComponentInParent<Canvas>() == null)
            return;

        go.transform.localPosition = Vector3.zero;
        Image image = go.AddComponent<Image>();
        image.raycastTarget = false;
        image.sprite = spriteRenderer.sprite;
        image.SetNativeSize();
        Object.DestroyImmediate(spriteRenderer);

        isTrigger = false;
    }
}