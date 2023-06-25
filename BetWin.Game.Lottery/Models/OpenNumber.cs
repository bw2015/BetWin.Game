using BetWin.Game.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Models
{
    /// <summary>
    /// 开奖号码
    /// </summary>
    public struct OpenNumber
    {
        /// <summary>
        /// 开奖号码
        /// </summary>
        private string openNumber;

        /// <summary>
        /// 开奖时间
        /// </summary>
        private long openTime;

        public OpenNumber(string openNumber, long openTime = 0)
        {
            if (openTime == 0) openTime = WebAgent.GetTimestamps();
            this.openNumber = openNumber;
            this.openTime = openTime;
        }

        public static implicit operator string(OpenNumber openNumber)
        {
            return openNumber.openNumber;
        }

        public static implicit operator OpenNumber(string openNumber)
        {
            return new OpenNumber(openNumber);
        }

        public static implicit operator DateTime(OpenNumber openNumber)
        {
            return new DateTime(openNumber.openTime);
        }
    }
}
