using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("火箭升空")]
    public class RocketUp : SmartBase
    {
        /// <summary>
        /// 水星
        /// </summary>
        const string Mercury = "6";

        /// <summary>
        /// 金星
        /// </summary>
        const string Venus = "4";

        /// <summary>
        /// 地球
        /// </summary>
        const string Earth = "3";

        /// <summary>
        /// 火星
        /// </summary>
        const string Mars = "5";

        /// <summary>
        /// 木星
        /// </summary>
        const string Jupiter = "8";

        /// <summary>
        /// 土星
        /// </summary>
        const string Saturn = "1";

        /// <summary>
        /// 天王星
        /// </summary>
        const string Uranus = "2";

        /// <summary>
        /// 海王星
        /// </summary>
        const string Neptune = "7";


        private string[] numbers = new[]
        {
            Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune
        };

        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            { Mercury,15 },
            { Venus,25 },
            { Earth,5 },
            { Mars,5 },
            { Jupiter,15 },
            { Saturn,5 },
            { Uranus,40 },
            { Neptune,5 }
        };


        protected override bool CheckBetContent(string content)
        {
            return numbers.Contains(content);
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return numbers.Contains((string)openNumber);
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            if (input == openNumber) return odds.GetOdds(input);
            return decimal.Zero;
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
