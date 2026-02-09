using System;
using System.Reflection;

public static class GFObjectExtension
{
    public static T GFGetFieldValue<T>(this object target, string fieldName, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance) where T : class
    {
        Type type = target.GetType();
        return type.GetField(fieldName, bindingAttr).GetValue(target) as T;
    }
}