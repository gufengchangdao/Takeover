using UnityEditor;

public static class DefineSymbolsMenus
{
    private static void Toggle(string symbol)
    {
        bool symbolExists = ScriptingDefineSymbols.HasScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup, symbol);
        if (symbolExists)
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(symbol);
        else
            ScriptingDefineSymbols.AddScriptingDefineSymbol(symbol);
    }

    private static void SetChecked(string menuPath, string symbol)
    {
        bool symbolExists = ScriptingDefineSymbols.HasScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup, symbol);
        Menu.SetChecked(menuPath, symbolExists);
    }

    #region HybridCLR
    private const string USE_HYBRIDCLR = "USE_HYBRIDCLR";
    private const string HYBRIDCLR_MENU_PATH = "Tools/Toggle HybridCLR Support";

    [MenuItem(HYBRIDCLR_MENU_PATH)]
    public static void ToggleHybridCLR()
    {
        Toggle(USE_HYBRIDCLR);
    }

    [MenuItem(HYBRIDCLR_MENU_PATH, true)]
    public static bool ValidateToggleHybridCLR()
    {
        SetChecked(HYBRIDCLR_MENU_PATH, USE_HYBRIDCLR);
        return true;
    }
    #endregion

    #region luban
    private const string USE_LUBAN = "USE_LUBAN";
    private const string LUBAN_MENU_PATH = "Tools/Toggle luban Support";

    [MenuItem(LUBAN_MENU_PATH)]
    public static void ToggleLuban()
    {
        Toggle(USE_LUBAN);
    }

    [MenuItem(LUBAN_MENU_PATH, true)]
    public static bool ValidateToggleLuban()
    {
        SetChecked(LUBAN_MENU_PATH, USE_LUBAN);
        return true;
    }
    #endregion
}