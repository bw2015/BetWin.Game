using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BetWin.Game.API.Utils
{
    internal static class WebAgent
    {
        public static long GetTimestamps()
        {
            return DateTime.Now.GetTimestamps();
        }

        public static long GetTimestamps(this DateTime time)
        {
            return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        public static DateTime GetTimestamps(long timestamp)
        {
            return new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset).AddMilliseconds(timestamp);
        }

        /// <summary>
        /// IPV4 转long
        /// </summary>
        /// <param name="ipV4"></param>
        /// <returns></returns>
        public static int ConvertIPv4(string ipV4)
        {
            if (!Regex.IsMatch(ipV4, @"^\d+\.\d+\.\d+\.\d+\$")) return 0;
            int ip = 0;
            string[] split = ipV4.Split('.');
            for (int i = 0; i < split.Length; i++)
            {
                string s = split[split.Length - i - 1];
                int x = int.Parse(s) << i * 8;
                ip |= x;
            }
            return ip;
        }

        /// <summary>
        /// 秒 时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static long GetTimestamp(DateTime dateTime)
        {
            return WebAgent.GetTimestamps(dateTime) / 1000L;
        }

        /// <summary>
        /// 获取时间戳（指定时区）
        /// </summary>
        internal static long GetTimestamps(DateTime time, TimeSpan offsetTime)
        {
            return (time.Subtract(offsetTime).Ticks - 621355968000000000) / 10000;
        }
    }
}
