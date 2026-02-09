using System;
using System.Collections.Generic;

namespace GameFramework.Hot
{
    /// <summary>
    /// 复合值
    /// 1. 值由多个值组合而成
    /// 2. 可以监听值的改变
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompositeValue<T>
    {
        private readonly Dictionary<string, T> valueDict = new();
        private T currentValue;
        private readonly object source;

        public delegate void OnValueChangedEvent(object source, T lastValue, T newValue);
        public event OnValueChangedEvent OnValueChanged;

        readonly Func<Dictionary<string, T>, T> Get;
        readonly Func<T, T, bool> IsEquip;

        public CompositeValue(object source, Func<Dictionary<string, T>, T> GetFn = null, Func<T, T, bool> IsEquipFn = null)
        {
            this.source = source;
            OPERATE_DICT.TryGetValue(typeof(T), out var fns);
            Get = GetFn ?? (Func<Dictionary<string, T>, T>)fns[0];
            IsEquip = IsEquipFn ?? (fns != null && fns.Length >= 1 ? (Func<T, T, bool>)fns[1] : null);
        }

        public T GetValue()
        {
            return currentValue;
        }

        public void Add(T value, string key)
        {
            var lastValue = currentValue;
            valueDict[key] = value;
            currentValue = Get(valueDict);

            if ((IsEquip != null && !IsEquip(lastValue, currentValue)) || (IsEquip == null && !Equals(lastValue, currentValue)))
                OnValueChanged?.Invoke(source, currentValue, lastValue);
        }

        public void Remove(string key)
        {
            var lastValue = currentValue;
            valueDict.Remove(key);
            currentValue = Get(valueDict);

            if ((IsEquip != null && !IsEquip(lastValue, currentValue)) || (IsEquip == null && !Equals(lastValue, currentValue)))
                OnValueChanged?.Invoke(source, currentValue, lastValue);
        }

        public bool IsEmpty()
        {
            return valueDict.Count <= 0;
        }

        #region 操作

        /// <summary>
        /// 操作字典，数组第一个参数为Get，第二个参数为IsEquip
        /// </summary>
        private static readonly Dictionary<Type, Delegate[]> OPERATE_DICT = new()
        {
            [typeof(float)] = new Delegate[] {
                (Func<Dictionary<string, float>, float>)Get_Float
            },
            [typeof(int)] = new Delegate[] {
                (Func<Dictionary<string, int>, int>)Get_Int
            },
            [typeof(bool)] = new Delegate[] {
                (Func<Dictionary<string, bool>, bool>)Get_Bool
            },
        };

        private static float Get_Float(Dictionary<string, float> dict)
        {
            float res = 0;
            foreach (var v in dict.Values)
                res += v;
            return res;
        }

        private static int Get_Int(Dictionary<string, int> dict)
        {
            int res = 0;
            foreach (var v in dict.Values)
                res += v;
            return res;
        }

        /// <summary>
        /// 同时为true的时候才为true，一个元素也没有的时候也为true
        /// </summary>
        private static bool Get_Bool(Dictionary<string, bool> dict)
        {
            foreach (var v in dict.Values)
                if (!v)
                    return false;
            return true;
        }

        #endregion
    }
}