using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("钢铁统帅")]
    public class Iron : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",8 }, {"2",4 }, {"3",2 }
        };

        public override string[] GetBetNumbers()
        {
            return new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        }

        protected override bool CheckBetContent(string content)
        {
            string[] betContent = content.Split(',').Distinct().ToArray();

            int count = betContent.Intersect(this.GetBetNumbers()).Count();
            return count >= 1 && count <= 3;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return this.GetBetNumbers().Contains(openNumber);
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            string[] betContent = input.Split(',').Distinct().ToArray();
            if (!betContent.Contains(openNumber)) return decimal.Zero;
            return odds.GetOdds(betContent.Length.ToString());

        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
