using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorSerializeFieldAttribute : Attribute
    {
        public string Key { get; }

        /// <summary>
        /// 保存和加载变量编辑器中的变量，使用EditorPrefs保存
        /// </summary>
        /// <param name="key">键，留空时默认为变量名，保存的时候会在前面自动加上类名</param>
        public EditorSerializeFieldAttribute(string key = null)
        {
            Key = key;
        }
    }

    [DrawerPriority(0.5)]
    public class EditorSerializeFieldAttributeDrawer : OdinAttributeDrawer<EditorSerializeFieldAttribute>
    {
        private string SaveKey => Application.productName + "." + Property.ParentType.Name + "." + (Attribute.Key ?? Property.Name);

        protected override void Initialize()
        {
            LoadValue();
            Property.ValueEntry.OnValueChanged += OnValueChanged;
        }

        private void LoadValue()
        {
            if (!EditorPrefs.HasKey(SaveKey))
                return;

            Type fieldType = Property.ValueEntry.TypeOfValue;
            if (fieldType == typeof(string))
                Property.ValueEntry.WeakSmartValue = EditorPrefs.GetString(SaveKey);
            else if (fieldType == typeof(int))
                Property.ValueEntry.WeakSmartValue = EditorPrefs.GetInt(SaveKey);
            else if (fieldType == typeof(float))
                Property.ValueEntry.WeakSmartValue = EditorPrefs.GetFloat(SaveKey);
            else if (fieldType == typeof(bool))
                Property.ValueEntry.WeakSmartValue = EditorPrefs.GetBool(SaveKey);
        }

        private void OnValueChanged(int state)
        {
            Type fieldType = Property.ValueEntry.TypeOfValue;
            string value = Property.ValueEntry.WeakSmartValue.ToString();
            if (fieldType == typeof(string))
                EditorPrefs.SetString(SaveKey, value);
            else if (fieldType == typeof(int))
                EditorPrefs.SetInt(SaveKey, int.Parse(value));
            else if (fieldType == typeof(float))
                EditorPrefs.SetFloat(SaveKey, float.Parse(value));
            else if (fieldType == typeof(bool))
                EditorPrefs.SetBool(SaveKey, bool.Parse(value));
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }
    }
}