using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Collects.Models
{
    /// <summary>
    /// 采集的数据
    /// </summary>
    public struct CollectData
    {
        public CollectData(string index, string number, long openTime)
        {
            this.Index = index;
            this.Number = number;
            this.OpenTime = openTime;
        }

        /// <summary>
        /// 彩期
        /// </summary>
        public string Index;

        /// <summary>
        /// 开奖时间
        /// </summary>
        public long OpenTime;

        /// <summary>
        /// 开奖号码
        /// </summary>
        public string Number;

        public static implicit operator bool(CollectData data)
        {
            return !string.IsNullOrEmpty(data.Index) && data.OpenTime != 0 && !string.IsNullOrEmpty(data.Number);
        }
    }
}
