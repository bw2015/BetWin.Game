using BetWin.Game.Lottery.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Models
{
    public struct RewardOdds
    {
        /// <summary>
        /// 中奖状态
        /// </summary>
        public RewardStatus Status;

        /// <summary>
        /// 中奖赔率
        /// </summary>
        public decimal Odds;

        public static implicit operator RewardOdds(decimal odds)
        {
            return new RewardOdds
            {
                Odds = odds,
                Status = odds switch
                {
                    0 => RewardStatus.Lose,
                    1 => RewardStatus.Return,
                    _ => RewardStatus.Win
                }
            };
        }

        public static implicit operator decimal(RewardOdds odds)
        {
            return Math.Round(odds.Odds, 4);
        }

    }
}
