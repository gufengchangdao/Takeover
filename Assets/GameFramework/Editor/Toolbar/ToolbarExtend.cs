using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class ToolbarExtend
{
    public static Action OnLeftToolbarGUI;
    public static Action OnRightToolbarGUI;

    private static readonly Type kToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    private static ScriptableObject sCurrentToolbar;


    static ToolbarExtend()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        if (sCurrentToolbar == null)
        {
            UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(kToolbarType);
            sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
            if (sCurrentToolbar != null)
            {
                FieldInfo root = sCurrentToolbar.GetType()
                    .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                VisualElement concreteRoot = root.GetValue(sCurrentToolbar) as VisualElement;

                VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                VisualElement parent = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };
                IMGUIContainer container = new IMGUIContainer();
                container.onGUIHandler -= OnRightGUI;
                container.onGUIHandler += OnRightGUI;
                parent.Add(container);
                toolbarZone.Add(parent);

                VisualElement toolbar = concreteRoot.Q("ToolbarZoneLeftAlign");
                VisualElement parent2 = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.RowReverse,
                    }
                };
                IMGUIContainer container2 = new IMGUIContainer();
                container2.onGUIHandler -= OnLeftGUI;
                container2.onGUIHandler += OnLeftGUI;
                parent2.Add(container2);
                toolbar.Add(parent2);

            }
        }
    }

    private static void OnLeftGUI()
    {
        GUILayout.BeginHorizontal();
        OnLeftToolbarGUI?.Invoke();
        GUILayout.EndHorizontal();
    }


    private static void OnRightGUI()
    {
        GUILayout.BeginHorizontal();
        OnRightToolbarGUI?.Invoke();
        GUILayout.EndHorizontal();
    }
}