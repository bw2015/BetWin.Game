using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
