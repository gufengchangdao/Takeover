using System;
using UnityEngine;

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
}