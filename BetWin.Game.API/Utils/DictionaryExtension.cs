using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Utils
{
    internal static class DictionaryExtension
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if(dictionary.ContainsKey(key)) return dictionary[key];
            return defaultValue;
        }
    }
}
