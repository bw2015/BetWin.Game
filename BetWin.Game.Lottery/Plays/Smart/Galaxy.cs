using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("银河探险")]
    public class Galaxy : SmartBase
    {
        /// <summary>
        ///  "水星" => "1",
        ///  "金星" => "2",
        ///  "火星" => "7",
        ///  "木星" => "8",
        ///  "土星" => "3",
        ///  "天王星" => "4",
        ///  "海王星" => "5",
        ///  "开普勒星" => "6", 
        /// </summary>
        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",5M },
            {"2",5M },
            {"3",10M },
            {"4",15M },
            {"5",25M },
            {"6",45M },
            {"7",5M },
            {"8",5M }
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
