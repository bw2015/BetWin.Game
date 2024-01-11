using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("抓鸭子")]
    internal class Duck : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",52 },
            {"2",32 },
            {"3",15M },
            {"4",8 },
            {"5",5 },
            {"6",6 },
            {"7",10 },
            {"8",3.2M }
        };

       

        protected override bool CheckBetContent(string content)
        {
            return DefaultOdds.GetOdds(content) != decimal.Zero;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return DefaultOdds.GetOdds(openNumber) != decimal.Zero;
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
