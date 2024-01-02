using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("动物运动会")]
    internal class AnimalRun : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",5 },
            {"2",5 },
            {"3",5 },
            {"4",5 },
            {"5",5 },
            {"6",5 }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.DefaultOdds.GetOdds(content) != decimal.Zero;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            string[] numbers = ((string)openNumber).Split(',').Distinct().ToArray();
            if (numbers.Length != 6) return false;
            if (numbers.Any(p => this.DefaultOdds.GetOdds(p) == decimal.Zero)) return false;
            return true;
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            string champion = ((string)openNumber).Split(',').FirstOrDefault();
            if (champion != input) return decimal.Zero;
            return odds.GetOdds(champion);
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
