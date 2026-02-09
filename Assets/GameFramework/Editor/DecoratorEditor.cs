using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A base class for creating editors that decorate Unity's built-in editor types.
/// </summary>
public abstract class DecoratorEditor : UnityEditor.Editor
{
    // empty array for invoking methods using reflection
    private static readonly object[] EMPTY_ARRAY = new object[0];

    #region Editor Fields

    /// <summary>
    /// Type object for the internally used (decorated) editor.
    /// </summary>
    private System.Type decoratedEditorType;

    private UnityEditor.Editor editorInstance;

    #endregion

    private static Dictionary<string, MethodInfo> decoratedMethods = new Dictionary<string, MethodInfo>();

    private static Assembly editorAssembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));

    protected UnityEditor.Editor EditorInstance
    {
        get
        {
            // if (editorInstance == null && targets != null && targets.Length > 0 && decoratedEditorType != null)
            // {
            //     editorInstance = UnityEditor.Editor.CreateEditor(targets, decoratedEditorType);
            // }

            if (editorInstance == null)
            {
                Debug.LogError("Could not create editor !");
            }

            return editorInstance;
        }
    }

    public DecoratorEditor()
    {
        decoratedEditorType = FindOtherEditorClassType(GetType());
    }

    private void OnEnable()
    {
        if (editorInstance == null && targets != null && targets.Length > 0 && decoratedEditorType != null)
        {
            editorInstance = UnityEditor.Editor.CreateEditor(targets, decoratedEditorType);
        }
    }

    private static Type FindOtherEditorClassType(Type targetType)
    {
        Type editorType = GetCustomEditorType(targetType);
        if (editorType == null)
            return null;

        List<Type> res = new();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type == targetType)
                    continue;

                Type otherEditorType = GetCustomEditorType(type);
                if (otherEditorType != editorType)
                    continue;

                res.Add(type);
            }
        }

        if (res.Count > 1)
        {
            Debug.LogWarning($"找到了多个{editorType.Name}的Editor类，请确认要使用哪个类 : {string.Join(", ", res)}");
            return res[0];
        }
        else if (res.Count <= 0)
        {
            Debug.LogWarning($"没有找到{editorType.Name}的Editor类");
            return null;
        }
        else
        {
            return res[0];
        }
    }

    private static Type GetCustomEditorType(Type type)
    {
        CustomEditor attribute = type.GetCustomAttribute(typeof(CustomEditor), false) as CustomEditor;
        if (attribute == null)
            return null;

        return attribute.GetType().GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(attribute) as Type;
    }

    void OnDisable()
    {
        if (editorInstance != null)
        {
            DestroyImmediate(editorInstance);
        }
    }

    /// <summary>
    /// Delegates a method call with the given name to the decorated editor instance.
    /// </summary>
    protected void CallInspectorMethod(string methodName)
    {
        if (EditorInstance == null)
            return;

        MethodInfo method;

        // Add MethodInfo to cache
        if (!decoratedMethods.ContainsKey(methodName))
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            method = decoratedEditorType.GetMethod(methodName, flags);
            if (method != null)
                decoratedMethods[methodName] = method;
        }
        else
        {
            method = decoratedMethods[methodName];
        }

        method?.Invoke(EditorInstance, EMPTY_ARRAY);
    }

    public void OnSceneGUI()
    {
        CallInspectorMethod("OnSceneGUI");
    }

    protected override void OnHeaderGUI()
    {
        CallInspectorMethod("OnHeaderGUI");
    }

    public override void OnInspectorGUI()
    {
        if (EditorInstance)
            EditorInstance.OnInspectorGUI();
    }

    public override void DrawPreview(Rect previewArea)
    {
        if (EditorInstance)
            EditorInstance.DrawPreview(previewArea);
    }

    public override string GetInfoString()
    {
        if (EditorInstance)
            return EditorInstance.GetInfoString();
        return "";
    }

    public override GUIContent GetPreviewTitle()
    {
        if (EditorInstance)
            return EditorInstance.GetPreviewTitle();
        return null;
    }

    public override bool HasPreviewGUI()
    {
        if (EditorInstance)
            return EditorInstance.HasPreviewGUI();
        return false;
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        if (EditorInstance)
            EditorInstance.OnInteractivePreviewGUI(r, background);
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        if (EditorInstance)
            EditorInstance.OnPreviewGUI(r, background);
    }

    public override void OnPreviewSettings()
    {
        if (EditorInstance)
            EditorInstance.OnPreviewSettings();
    }

    public override void ReloadPreviewInstances()
    {
        if (EditorInstance)
            EditorInstance.ReloadPreviewInstances();
    }

    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        if (EditorInstance)
            return EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);
        else
            return null;
    }

    public override bool RequiresConstantRepaint()
    {
        if (EditorInstance)
            return EditorInstance.RequiresConstantRepaint();
        return false;
    }

    public override bool UseDefaultMargins()
    {
        if (EditorInstance)
            return EditorInstance.UseDefaultMargins();
        return false;
    }
}