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
    }
}
