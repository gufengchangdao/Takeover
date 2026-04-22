using System;
using TMPro;

namespace GameFramework.Hot
{
    /// <summary>
    /// 支持多语言的文本组件
    /// 组件初始化和语言发生改变时会根据文本内容查表判断是否有对应的多语言文本，然后自动替换内容和字体
    /// 事件OnTextChange可以监听文本改变
    /// </summary>
    public class GFText : TextMeshProUGUI
    {
        public override string text
        {
            get => base.text;
            set
            {
                var oldText = text;
                base.text = value;

                if (oldText != value)
                {
                    if (!multLanguageChanging)
                        tableKey = null; //文本被修改，不再查表自动更新文本

                    OnTextChange.InvokeSafe(value, oldText);
                }
            }
        }

        public event Action<string, string> OnTextChange;

        private string materialName;
        private bool multLanguageChanging;
        private string tableKey;

        protected override void Awake()
        {
            base.Awake();

            if (!gameObject.IsRuntimeSceneObject())
                return;

            materialName = fontSharedMaterial.name;
            if (GFGlobal.Tables.TbMultiLanguageText.GetOrDefault(text) != null)
                tableKey = text;
        }

        protected override void Start()
        {
            base.Start();

            if (!gameObject.IsRuntimeSceneObject())
                return;

            OnLanguageChange();
        }

        protected override void OnDestroy()
        {
            OnTextChange = null;
            base.OnDestroy();
        }

        private void UpdateFont()
        {
            GFFontManager.Instance.GetFontMaterial(materialName, out var font, out var material);
            // 字体
            this.font = font;
            // 材质
            fontSharedMaterial = material;
        }

        public void OnLanguageChange()
        {
            UpdateFont();
            UpdateMultLanguageText();
        }

        private void UpdateMultLanguageText()
        {
            if (tableKey == null)
                return;

            var textData = GFGlobal.Tables.TbMultiLanguageText.GetOrDefault(tableKey);
            if (textData != null)
            {
                multLanguageChanging = true;
                text = textData.Text;
                multLanguageChanging = false;
            }
        }
    }
}