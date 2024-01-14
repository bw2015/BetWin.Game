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
            {"1",4 },
            {"2",5 },
            {"3",40 },
            {"4",30 },
            {"5",12 },
            {"6",11 },
            {"7",9 },
            {"8",2.5M }
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
