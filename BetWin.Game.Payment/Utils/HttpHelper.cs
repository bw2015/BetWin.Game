using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetWin.Game.Payment.Utils
{
    internal static class HttpHelper
    {
        public static string GetString(this HttpContext context, Encoding? encoding = null)
        {
            if (context.Request.Method != "POST" || context.Request.ContentLength == null) return string.Empty;
            encoding ??= Encoding.UTF8;
            using (MemoryStream ms = new MemoryStream((int)context.Request.ContentLength))
            {
                context.Request.Body.CopyToAsync(ms).Wait();
                ms.Position = 0;
                byte[] data = ms.ToArray();
                return encoding.GetString(data);
            }
        }
    }
}
