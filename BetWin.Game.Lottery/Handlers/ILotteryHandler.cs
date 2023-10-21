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
        public void SaveStepTime(string lotteryCode, StepTimeModel stepTime);

        /// <summary>
        /// 保存自定义的内容
        /// </summary>
        /// <param name="lotteryCode">彩种代码</param>
        /// <param name="index">彩期</param>
        /// <param name="value">自定义对象</param>
        public void SaveIndexData(string lotteryCode, string index, object value);
        BetOrderResult GetOrderResult(string lotteryCode);
    }
}
