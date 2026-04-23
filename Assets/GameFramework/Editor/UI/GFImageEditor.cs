using GameFramework.Hot;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(GFImage), true)]
[CanEditMultipleObjects]
public class GFImageEditor : ImageEditor
{
    private SerializedProperty statuses;

    protected override void OnEnable()
    {
        base.OnEnable();

        statuses = serializedObject.FindProperty("statuses");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.UpdateIfRequiredOrScript();
        EditorGUILayout.PropertyField(statuses, true);
        serializedObject.ApplyModifiedProperties();
    }
}