using System;
using System.Collections.Generic;

namespace GameFramework.Hot
{
    /// <summary>
    /// 监听值的改变
    /// </summary>
    public class BindableProperty<T>
    {
        private T m_Value;
        public T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                // 值没变时不触发事件
                if (value == null && m_Value == null) return;
                if (value != null)
                    if (value.Equals(m_Value)) return; //默认比较器

                var oldValue = m_Value;
                m_Value = value;
                OnChange?.InvokeSafe(value, oldValue);
            }
        }

        public event Action<T, T> OnChange;

        public BindableProperty(T defaultValue = default, Action<T, T> onValueChange = null)
        {
            m_Value = defaultValue;
            OnChange = onValueChange;
        }

        public void ClearAllEvents()
        {
            OnChange = null;
        }
    }

    public class DictionaryProperty<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_Dictionary = new();

        public event Action<TKey, TValue> OnAdd;
        public event Action<TKey, TValue> OnRemove;

        public TValue this[TKey key] => m_Dictionary[key];

        public DictionaryProperty(Action<TKey, TValue> OnAdd = null, Action<TKey, TValue> OnRemove = null)
        {
            this.OnAdd = OnAdd;
            this.OnRemove = OnRemove;
        }

        public void Add(TKey key, TValue value)
        {
            m_Dictionary.Add(key, value);
            OnAdd?.InvokeSafe(key, value);
        }

        public bool Remove(TKey key)
        {
            if (m_Dictionary.Remove(key))
            {
                OnRemove?.InvokeSafe(key, m_Dictionary[key]);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            foreach (var item in m_Dictionary)
                OnRemove?.InvokeSafe(item.Key, item.Value);
            m_Dictionary.Clear();
        }

        public void ClearAllEvents()
        {
            OnAdd = null;
            OnRemove = null;
        }
    }
}