using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Core.Collections
{
    /// <summary>
    /// 字典扩展
    /// </summary>
    public static class DictionaryExtendssion
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> data, TKey key, TValue defaultValue = default)
        {
            if (data.ContainsKey(key)) return data[key];
            return defaultValue;
        }
    }
}
