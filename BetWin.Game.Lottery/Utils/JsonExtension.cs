using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Utils
{
    internal static class JsonExtension
    {
        public static T? ToJson<T>(this string content) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return null;
            }
        }

        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
