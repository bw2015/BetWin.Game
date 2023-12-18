using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("西游夺宝")]
    internal class XiYou : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            //悟空
            {"1",30M },
            //八戒
            {"2",5M },
            //悟净
            {"3",4M },
            //龙马
            {"4",2M }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.DefaultOdds.GetOdds(content) != decimal.Zero;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            string[] numbers = ((string)openNumber).Split(',');
            if (!numbers.Any()) return false;
            return !numbers.Any(t => this.DefaultOdds.GetOdds(t) == decimal.Zero);
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            string[] numbers = ((string)openNumber).Split(',');
            if (numbers.Contains(input)) return odds.GetOdds(input);
            return decimal.Zero;
        }

        protected override int GetBetCountNumber(string input)
        {
            return this.CheckBetContent(input) ? 1 : 0;
        }
    }
}
