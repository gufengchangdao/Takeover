using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.AOT
{
    /// <summary>
    /// 用于热更新前的Text使用
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class SimpleMultiLanguageText : MonoBehaviour
    {
        [SerializeField]
        private List<TextData> textDatas = new List<TextData>();

        void Awake()
        {
            var text = GetComponent<Text>();
            foreach (var data in textDatas)
            {
                if (data.language == AOTGameConfig.Language)
                {
                    text.text = data.text;
                    return;
                }
            }
        }

        [Serializable]
        private class TextData
        {
            public SystemLanguage language;
            public string text;
        }
    }
}