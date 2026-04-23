using GameFramework.Hot;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class UICreateMenu
{
    private static T CreateGameObject<T>(string name, MenuCommand menuCommand)
    {
        var parent = menuCommand.context as GameObject;
        var gameObject = new GameObject(name, typeof(T));
        GameObjectUtility.SetParentAndAlign(gameObject, parent);
        Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");
        Selection.activeGameObject = gameObject;
        return gameObject.GetComponent<T>();
    }

    [MenuItem("GameObject/UI/GFText", false, -10)]
    private static void CreateGFText(MenuCommand menuCommand)
    {
        var text = CreateGameObject<GFText>("Txt", menuCommand);
        text.text = "New Text";
        text.raycastTarget = false;
        text.alignment = TextAlignmentOptions.Center;
    }

    [MenuItem("GameObject/UI/GFImage", false, -10)]
    private static void CreateGFImage(MenuCommand menuCommand)
    {
        var image = CreateGameObject<GFImage>("Img", menuCommand);
        image.raycastTarget = false;
    }
}