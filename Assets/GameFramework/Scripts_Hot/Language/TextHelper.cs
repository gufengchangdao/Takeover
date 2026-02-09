using TMPro;
using UnityEngine;

namespace GameFramework.Hot
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextHelper : MonoBehaviour
    {
        /// <summary>
        /// 字体类型，只在初始化时赋值，不支持修改，所以文本组件都不应该用AddComponent添加，而且做好预制件再加载
        /// 语言更新时更改字体、材质和内容，如果文本被外部修改过则不再自动替换文本
        /// </summary>
        private string materialName;
        private TextMeshProUGUI target;
        private string key;
        private string curText;

        private bool HasChanged => curText != target.text;

        private string Text
        {
            get
            {
                return target.text;
            }
            set
            {
                target.text = value; //记录当前文本，用于比较是否文本是否被修改过
                curText = value;
            }
        }

        void Awake()
        {
            target = GetComponent<TextMeshProUGUI>();
            materialName = target.fontSharedMaterial.name;

            UpdateFont();

            string k = Text;
            if (TrySetTableText(k))
                key = k;
        }

        private void UpdateFont()
        {
            GFFontManager.Instance.GetFontMaterial(materialName, out var font, out var material);
            // 字体
            target.font = font;
            // 材质
            target.fontSharedMaterial = material;
        }

        public void OnLanguageChange()
        {
            if (key == null)
                return; //不是多语言文本

            if (HasChanged) //文本被外部修改过，不需要再自动替换文本
            {
                curText = null;
                return;
            }

            UpdateFont();

            if (!TrySetTableText(key))
                Text = key;
        }

        private bool TrySetTableText(string key)
        {
            var textData = GFGlobal.Tables.TbMultiLanguageText.GetOrDefault(key);
            if (textData != null)
            {
                Text = textData.Text;
                return true;
            }
            return false;
        }
    }
}