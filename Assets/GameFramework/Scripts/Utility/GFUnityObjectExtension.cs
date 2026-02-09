using UnityEngine;

namespace GameFramework.AOT
{
    public static class GFUnityObjectExtension
    {
        public static void ResetInstantiateName(this Object obj, Object source)
        {
            string content;
            string s = source.name.Substring(source.name.LastIndexOf("_") + 1);
            if (int.TryParse(s, out int suffix))
            {
                suffix++;
                content = source.name.Substring(0, source.name.LastIndexOf("_"));
            }
            else
            {
                suffix = 1;
                content = source.name;
            }
            obj.name = content + "_" + suffix;
        }
    }
}