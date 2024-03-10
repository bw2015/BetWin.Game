using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("末日保卫战")]
    public class Doomsday : SmartBase
    {
        public Doomsday()
        {
        }

        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {

            {"1",5 },    // 喷火枪
            {"2",5 },   // 投石器
            {"3",35 },  // 猎龙驽
            {"4",25 },  // 太阳权杖
            {"5",10 },  // 铁蒺藜
            {"6",10 },  // 剧毒屏障
            {"7",5 },   // 弩机
            {"8",5 }    // 圣光之心
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
            if (input == openNumber)
            {
                return odds.GetOdds(input);
            }
            return decimal.Zero;
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
