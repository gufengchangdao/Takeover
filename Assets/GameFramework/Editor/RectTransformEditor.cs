using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(RectTransform), true)]
[CanEditMultipleObjects]
public partial class RectTransformEditor : DecoratorEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);

        var rectTransform = (RectTransform)target;
        if (targets.Length == 1 && rectTransform.GetComponent<Image>())
            if (GUILayout.Button("调整Pivot以使局部坐标归零"))
                ZeroLocalPositionWithPivotAdjustment(rectTransform);

        if (targets.Length > 1)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("对齐");
            var hIcon = EditorGUIUtility.IconContent("d_align_horizontally_center");
            hIcon.tooltip = "水平居中";
            var vIcon = EditorGUIUtility.IconContent("d_align_vertically_center");
            vIcon.tooltip = "垂直居中";
            if (GUILayout.Button(hIcon, GUILayout.Width(28), GUILayout.Height(28)))
                SetGameObjectCenter(true);
            if (GUILayout.Button(vIcon, GUILayout.Width(28), GUILayout.Height(28)))
                SetGameObjectCenter(false);
            GUILayout.EndHorizontal();
        }

        if (targets.Length == 2)
        {
            if (GUILayout.Button("拷贝对象相对路径"))
                CopyRelativePath();
        }
    }

    public void ZeroLocalPositionWithPivotAdjustment(RectTransform rectTransform)
    {
        Vector2 pos = rectTransform.localPosition;
        Rect rect = rectTransform.rect;
        Vector2 scale = rectTransform.localScale;
        if (rect.width == 0 || rect.height == 0 || scale.x == 0 || scale.y == 0)
        {
            Debug.LogWarning(rectTransform.name + "的RectTransform的宽高或缩放为0，无法调整Pivot。");
            return;
        }

        Vector2 offset = new(-pos.x / rect.width / scale.x, -pos.y / rect.height / scale.y);
        rectTransform.pivot += offset;
        rectTransform.localPosition = Vector3.zero;
    }

    private void SetGameObjectCenter(bool isHorizontal)
    {
        List<(RectTransform, float)> list = new();
        foreach (var t in targets)
        {
            var rectTransform = (RectTransform)t;
            list.Add((rectTransform, isHorizontal ? rectTransform.localPosition.x : rectTransform.localPosition.y));
        }
        list.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        float gap = (list[list.Count - 1].Item2 - list[0].Item2) / (list.Count - 1);
        float cur = list[0].Item2;
        for (int i = 1; i < list.Count; i++)
        {
            cur += gap;
            if (isHorizontal)
                list[i].Item1.localPosition = new Vector3(cur, list[i].Item1.localPosition.y);
            else
                list[i].Item1.localPosition = new Vector3(list[i].Item1.localPosition.x, cur);
        }
    }

    private void CopyRelativePath()
    {
        RectTransform go1 = (RectTransform)targets[0];
        RectTransform go2 = (RectTransform)targets[1];

        RectTransform parent = null;
        RectTransform child = null;

        if (go1.IsChildOf(go2))
        {
            parent = go2;
            child = go1;
        }
        else if (go2.IsChildOf(go1))
        {
            parent = go1;
            child = go2;
        }

        if (parent == null)
            return; //两个对象不是父子级

        string path = GetPath(child, parent);
        EditorGUIUtility.systemCopyBuffer = path;
    }

    public static string GetPath(Transform transform, Transform parent = null)
    {
        string s = transform.name;
        while (transform.parent != parent)
        {
            transform = transform.parent;
            s = $"{transform.name}/{s}";
        }
        return s;
    }
}