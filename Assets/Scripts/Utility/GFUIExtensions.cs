
using GameFramework.AOT;
using GameFramework.Hot;

public static class GFUIExtensions
{
    public static void OpenPanel<C>(this GFUI UI, string name, int guid = 0, object userData = null) where C : BaseControl
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

        UI.OpenPanel<C>(data.Group, path, guid, userData);
    }
}