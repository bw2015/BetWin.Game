using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Enums
{
    /// <summary>
    /// 中奖状态
    /// </summary>
    public enum RewardStatus : byte
    {
        /// <summary>
        /// 退还本金
        /// </summary>
        Return = 1,
        /// <summary>
        /// 中奖
        /// </summary>
        Win,
        /// <summary>
        /// 未中奖
        /// </summary>
        Lose
    }
}
