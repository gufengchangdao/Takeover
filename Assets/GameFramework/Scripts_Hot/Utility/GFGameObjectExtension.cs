using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public static class GFGameObjectExtension
{
    /// <summary>
    /// 获取或增加组件。
    /// </summary>
    /// <typeparam name="T">要获取或增加的组件。</typeparam>
    /// <param name="go">目标对象。</param>
    /// <returns>获取或增加的组件。</returns>
    public static T GFGetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return go.TryGetComponent(out T val) ? val : go.AddComponent<T>();
    }

    public static Component GFGetOrAddComponent(this GameObject go, Type type)
    {
        return go.TryGetComponent(type, out var component) ? component : go.AddComponent(type);
    }

    public static T GFFindComponent<T>(this GameObject go, string path = null) where T : Component
    {
        return go.transform.GFFindComponent<T>(path);
    }

    /// <summary>
    /// 判断GameObject是否是运行下游戏场景里的游戏对象
    /// </summary>
    public static bool IsRuntimeSceneObject(this GameObject go)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return false;

        if (!go.scene.IsValid() || !go.scene.isLoaded)
            return false;

        if (EditorSceneManager.IsPreviewSceneObject(go))
            return false;
#endif
        return true;
    }
}