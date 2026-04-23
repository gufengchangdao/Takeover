// using GameFramework.Hot;
// using TMPro;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// public static class GFTextMenu
// {
//     [MenuItem("GameObject/UI/GFText", false, 2031)]
//     private static void CreateGFText(MenuCommand menuCommand)
//     {
//         var parent = GetOrCreateParent(menuCommand);
//         var gameObject = new GameObject("GFText", typeof(RectTransform), typeof(CanvasRenderer), typeof(GFText));

//         Undo.RegisterCreatedObjectUndo(gameObject, "Create GFText");
//         SetParentAndAlign(gameObject, parent);
//         gameObject.layer = parent != null ? parent.layer : LayerMask.NameToLayer("UI");

//         var rectTransform = gameObject.GetComponent<RectTransform>();
//         rectTransform.sizeDelta = new Vector2(160f, 30f);

//         var text = gameObject.GetComponent<GFText>();
//         text.text = "New Text";
//         text.raycastTarget = false;
//         text.fontSize = 36f;
//         text.color = Color.white;
//         text.alignment = TextAlignmentOptions.Center;

//         if (TMP_Settings.defaultFontAsset != null)
//             text.font = TMP_Settings.defaultFontAsset;

//         Selection.activeGameObject = gameObject;
//     }

//     private static GameObject GetOrCreateParent(MenuCommand menuCommand)
//     {
//         var contextObject = menuCommand.context as GameObject;
//         var parent = GetParentFromContext(contextObject) ?? FindAnyCanvas();
//         if (parent != null)
//             return parent;

//         return CreateCanvas();
//     }

//     private static GameObject GetParentFromContext(GameObject contextObject)
//     {
//         if (contextObject == null)
//             return null;

//         if (contextObject.GetComponentInParent<Canvas>() == null)
//             return null;

//         return contextObject.GetComponent<RectTransform>() != null
//             ? contextObject
//             : FindCanvasInParents(contextObject);
//     }

//     private static GameObject FindCanvasInParents(GameObject gameObject)
//     {
//         var canvas = gameObject.GetComponentInParent<Canvas>();
//         return canvas != null ? canvas.gameObject : null;
//     }

//     private static GameObject FindAnyCanvas()
//     {
//         var canvas = Object.FindFirstObjectByType<Canvas>();
//         return canvas != null ? canvas.gameObject : null;
//     }

//     private static GameObject CreateCanvas()
//     {
//         var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
//         Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");

//         var canvas = canvasObject.GetComponent<Canvas>();
//         canvas.renderMode = RenderMode.ScreenSpaceOverlay;
//         canvasObject.layer = LayerMask.NameToLayer("UI");

//         var scaler = canvasObject.GetComponent<CanvasScaler>();
//         scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//         scaler.referenceResolution = new Vector2(1920f, 1080f);
//         scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

//         CreateEventSystemIfNeeded();
//         return canvasObject;
//     }

//     private static void CreateEventSystemIfNeeded()
//     {
//         if (Object.FindFirstObjectByType<EventSystem>() != null)
//             return;

//         var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
//         Undo.RegisterCreatedObjectUndo(eventSystemObject, "Create EventSystem");
//     }

//     private static void SetParentAndAlign(GameObject child, GameObject parent)
//     {
//         if (parent == null)
//             return;

//         Undo.SetTransformParent(child.transform, parent.transform, "Parent GFText");
//         GameObjectUtility.SetParentAndAlign(child, parent);
//     }
// }
