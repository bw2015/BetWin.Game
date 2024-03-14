using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Utils
{
    internal static class JsonHelper
    {
        public static string ToJson(this object info)
        {
            return JsonConvert.SerializeObject(info, Formatting.Indented);
        }

        public static T? ToJson<T>(this string jsonString) where T : class 
        {
            if (string.IsNullOrEmpty(jsonString)) return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return default;
            }
        }
    }
}
