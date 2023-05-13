using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BetWin.Game.API.Utils
{
    public static class JsonExtensions
    {
        /// <summary>
        /// 转化成为json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, JsonSerializerSettingConfig.Setting);
        }

        /// <summary>
        /// 字符串反序列化成为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) return default;
            if (!json.StartsWith("{") && !json.StartsWith("[")) return default;
            return JsonConvert.DeserializeObject<T>(json);
        }


        /// <summary>
        /// 获取Map对象值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this JObject info, string key)
        {
            if (!info.ContainsKey(key)) return default;
            JToken? token = info[key];
            if (token == null || token.GetType() != typeof(JValue)) return default;
            JValue value = (JValue)token;
            if (value.Value == null) return default;
            return value.Value<T>();
        }

        /// <summary>
        /// 更新JSON对象的一个对象(自身内部更新）
        /// </summary>
        /// <param name="info"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static JObject Update(this JObject info, string field, JToken value)
        {
            JToken? token = info[field];
            token?.Replace(value);
            return info;
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
              (token.Type == JTokenType.Array && !token.HasValues) ||
              (token.Type == JTokenType.Object && !token.HasValues) ||
              (token.Type == JTokenType.String && token.Value<string>() == string.Empty) ||
              (token.Type == JTokenType.Null);
        }
    }
}
