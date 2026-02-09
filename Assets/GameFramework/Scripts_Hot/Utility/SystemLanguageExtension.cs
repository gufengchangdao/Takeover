using System.Collections.Generic;
using UnityEngine;

public static class SystemLanguageExtension
{
    private static Dictionary<SystemLanguage, string> s_LanguageCodeMap = new()
    {
        [SystemLanguage.Arabic] = "ar",
        [SystemLanguage.German] = "de",
        [SystemLanguage.English] = "en",
        [SystemLanguage.Spanish] = "es",
        [SystemLanguage.French] = "fr",
        [SystemLanguage.Indonesian] = "id",
        [SystemLanguage.Italian] = "it",
        [SystemLanguage.Japanese] = "ja",
        [SystemLanguage.Korean] = "ko",
        [SystemLanguage.Portuguese] = "pt",
        [SystemLanguage.Russian] = "ru",
        [SystemLanguage.Thai] = "th",
        [SystemLanguage.Turkish] = "tr",
        [SystemLanguage.Vietnamese] = "vi",
        [SystemLanguage.ChineseSimplified] = "zh-Hans",
        [SystemLanguage.ChineseTraditional] = "zh-Hant",
    };

    public static string GFGetCode(this SystemLanguage language)
    {
        if (s_LanguageCodeMap.TryGetValue(language, out var code))
            return code;
        return "zh-Hans"; //默认中文简体
    }
}