using System;
using UnityEngine;
using YooAsset;

public static class AOTGameConfig
{
    public const SystemLanguage DefaultLanguage = SystemLanguage.ChineseSimplified;

    public static SystemLanguage s_Language = Application.systemLanguage;
    public static event Action<SystemLanguage> OnLanguageChange;
    public static SystemLanguage Language
    {
        get => s_Language;
        set
        {
            if (s_Language == value)
                return;

            var old = s_Language;
            s_Language = value;
            OnLanguageChange?.Invoke(s_Language);
        }
    }

    public static EPlayMode PlayMode { get; set; }
}
