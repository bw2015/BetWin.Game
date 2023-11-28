using BetWin.Game.Lottery.Base;
using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("三国乱斗")]
    internal class King3 : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            // 魏
            {"1", 1.5M},
            // 蜀
            {"2",2.5M },
            // 吴
            {"3",3.5M },
            // 蛮
            {"4",15M },
            // 羌
            {"5",35M }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.DefaultOdds.GetOdds(content) != decimal.Zero;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return this.DefaultOdds.GetOdds(openNumber) != decimal.Zero;
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            if (input != openNumber) return decimal.Zero;
            return odds.GetOdds(input);
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
