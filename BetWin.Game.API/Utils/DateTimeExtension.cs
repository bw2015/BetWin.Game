using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Utils
{
    internal static class DateTimeExtension
    {
        /// <summary>
        /// 获取时区时间
        /// </summary>
        /// <param name="datetime">当前服务器时区的时间</param>
        /// <param name="timezone">要计算的时区</param>
        /// <returns></returns>
        public static DateTime GetTimeZone(this DateTime datetime, int timezone)
        {
            //#1 得到UTC-0 的时间
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(datetime);

            //#2 得到偏差值
            return utcTime.AddHours(timezone);
        }
    }
}
