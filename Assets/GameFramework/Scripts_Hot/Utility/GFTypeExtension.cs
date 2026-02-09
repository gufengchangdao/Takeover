using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.AOT;

public static class GFTypeExtension
{
    public static List<Type> GFGetChildType(this Type type)
    {
        var results = new List<Type>();
#if UNITY_EDITOR
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = assembly.GetTypes();
            foreach (var t in types)
            {
                if ((t.IsSubclassOf(type) || type.IsAssignableFrom(t))
                        && !t.IsAbstract
                        && !t.IsInterface)
                    results.Add(t);
            }
        }
#else
        Log.Error("GFGetChildTypes should not be called at runtime");
#endif
        return results;
    }

    /// <summary>
    /// 拿到这个类型的子类，用于编辑器里选择用的
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<string> GFGetChildTypeName(this Type type)
    {
        return type.GFGetChildType().Select(t => t.FullName).ToList();
    }
}