
using System;
using GameFramework.AOT;
using GameFramework.Hot;

public static class GFUIExtensions
{
    private const string ControlSuffix = "Control";

    public static void OpenPanel<C>(this GFUI UI, int guid = 0, object userData = null) where C : BaseControl
    {
        string name = typeof(C).Name;
        if (name.EndsWith(ControlSuffix, StringComparison.Ordinal))
            name = name.Substring(0, name.Length - ControlSuffix.Length);

        UI.OpenPanel(typeof(C), name, guid, userData);
    }

    public static void OpenPanel<C>(this GFUI UI, string name, int guid = 0, object userData = null) where C : BaseControl
    {
        UI.OpenPanel(typeof(C), name, guid, userData);
    }

    public static void OpenPanel(this GFUI UI, Type controlType, string name, int guid = 0, object userData = null)
    {
        var data = GFGlobal.Tables.TbPanelData.GetOrDefault(name);
        if (data == null)
        {
            Log.Error("[UI] Panel not found: {0}", name);
            return;
        }

        // 界面预制件默认路径
        string path = data.AssetPath;
        if (string.IsNullOrEmpty(path))
            path = string.Format(GFGlobal.Tables.TbGlobalSettingData.DefaultPanelPath, name);

        UI.OpenPanel(controlType, data.Group, path, guid, userData);
    }
}
