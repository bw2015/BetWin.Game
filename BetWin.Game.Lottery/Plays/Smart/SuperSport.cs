using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("超能运动会")]
    public class SuperSport : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",2 },
            {"2",5 },
            {"3",5 },
            {"4",10 },
            {"5",25 },
            {"6",50 }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.DefaultOdds.GetOdds(content) != decimal.Zero;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return this.CheckBetContent(openNumber);
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            if (input != openNumber) return decimal.Zero;
            return odds.GetOdds(input);
        }

        protected override int GetBetCountNumber(string input)
        {
            return this.CheckBetContent(input) ? 1 : 0;
        }
    }
}
