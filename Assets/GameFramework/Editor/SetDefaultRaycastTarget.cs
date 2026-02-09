using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class SetDefaultRaycastTarget
{
    static SetDefaultRaycastTarget()
    {
        // Debug.Log("<color=#888888>开始监听新增组件，Image和TextMeshProUGUI的RaycastTarget默认为false</color>");
        ObjectFactory.componentWasAdded -= HandleComponentAdded;
        ObjectFactory.componentWasAdded += HandleComponentAdded;
    }

    private static void HandleComponentAdded(Component component)
    {
        if (component is Image image)
        {
            image.raycastTarget = false;
            EditorUtility.SetDirty(image);
            return;
        }

        if (component is TextMeshProUGUI text)
        {
            text.raycastTarget = false;
            EditorUtility.SetDirty(text);
            return;
        }
    }
}