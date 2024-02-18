using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BetWin.Game.Lottery.Utils
{
    internal static class DictionaryExtension
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return default;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return defaultValue;
        }

        /// <summary>
        /// 转化成为QueryString格式
        /// </summary>
        /// <param name="urlEncode">Value内容是否使用Url编码</param>
        public static string ToQueryString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary, bool urlEncode = false)
        {
            return string.Join("&", dictionary.Select(t => $"{t.Key}={(urlEncode ? HttpUtility.UrlEncode(t.Value == null ? string.Empty : t.Value.ToString()) : t.Value?.ToString())}"));
        }


    }
}
