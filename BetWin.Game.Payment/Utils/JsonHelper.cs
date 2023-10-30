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
    }
}
