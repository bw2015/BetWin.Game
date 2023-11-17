using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Handlers
{
    /// <summary>
    /// 外部注入的接口类
    /// </summary>
    public interface ILotteryHandler
    {
        public void SaveIndexTime(int lotteryId, StepTimeModel stepTime);

        /// <summary>
        /// 保存自定义的内容
        /// </summary>
        /// <param name="index">彩期</param>
        /// <param name="value">自定义对象</param>
        public void SaveIndexData(int lotteryId, string index, object value);

        BetOrderResult GetOrderResult(int lotteryId);

        /// <summary>
        /// 获取连续的期号
        /// </summary>
        /// <param name="period">两期之间开奖时间的最低间隔（秒）</param>
        string GetBetIndex(int lotteryCode, long openTime, int period);
    }
}
