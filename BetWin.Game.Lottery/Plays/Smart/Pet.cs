using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("宠物马拉松")]
    public class Pet : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",decimal.Zero },
            {"2",decimal.Zero },
            {"3",decimal.Zero },
            {"4",decimal.Zero },
            {"5",decimal.Zero },
            {"6",decimal.Zero },
            {"7",decimal.Zero }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.DefaultOdds.Contains(content);
        }

        /// <summary>
        /// 检查开奖号码是否
        /// </summary>
        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            Dictionary<string, decimal>? odds = this.handler?.GetIndexData<Dictionary<string, decimal>>(openNumber.lotteryId, openNumber.index);
            if (odds == null) return false;
            return odds?.Get(openNumber) != decimal.Zero;
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            Dictionary<string, decimal>? petOdds = this.handler?.GetIndexData<Dictionary<string, decimal>>(openNumber.lotteryId, openNumber.index);
            if (petOdds == null) return decimal.Zero;
            if (input != openNumber) return decimal.Zero;

            return petOdds.Get(openNumber);
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
