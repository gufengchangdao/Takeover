using UnityEngine;

namespace GameFramework.Hot
{
    public static class LanguageUtility
    {
        // 映射为支持的语言
        public static SystemLanguage GetSupportedLanguage(SystemLanguage language)
        {
            if (language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional || language == SystemLanguage.Chinese)
                return SystemLanguage.ChineseSimplified;

            return SystemLanguage.English;
        }
    }
}



