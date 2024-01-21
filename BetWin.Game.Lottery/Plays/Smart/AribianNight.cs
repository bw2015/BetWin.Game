using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("一千零一夜")]
    internal class AribianNight : SmartBase
    {
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",39 },
            {"2",66 },
            {"3",55 },
            {"4",21 },
            {"5",16 },
            {"6",12 },
            {"7",10 },
            {"8",9 },
            {"9",8 },
            {"10",7 },
            {"11",5 },
            {"12",6 }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.DefaultOdds.GetOdds(content) > decimal.Zero;
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            string[] numbers = ((string)openNumber).Split(',');

            foreach (string number in numbers)
            {
                if (!this.CheckBetContent(number))
                {
                    return false;
                }
            }
            return true;
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
