using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    /// <summary>
    /// 小红帽
    /// </summary>
    public class RedHat : SmartBase
    {
        /// <summary>
        /// 狐狸
        /// </summary>
        const string FOX = "1";
        /// <summary>
        /// 猎狗
        /// </summary>
        const string DOG = "2";
        /// <summary>
        /// 魔牛
        /// </summary>
        const string OX = "3";
        /// <summary>
        /// 疯羊
        /// </summary>
        const string SHEEP = "4";
        /// <summary>
        /// 毒蛇
        /// </summary>
        const string SNAKE = "5";

        private string[] NUMBERS = new[]
        {
            FOX, DOG, OX, SHEEP, SNAKE
        };

        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            { FOX, 2 },
            { DOG,2 },
            { OX,32 },
            { SHEEP,12 },
            { SNAKE,4 }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.NUMBERS.Contains(content);
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return this.NUMBERS.Contains((string)openNumber);
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            if (openNumber == input)
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
