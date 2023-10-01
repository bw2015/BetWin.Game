using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Plays.Smart
{
    [Description("诺亚方舟")]
    public class NoahArk : SmartBase
    {
        #region ======== 投注选项  ========

        /// <summary>
        /// 水怪
        /// </summary>
        const string 喷火器 = "1";

        /// <summary>
        /// 蝙蝠
        /// </summary>
        const string 激光塔 = "2";

        /// <summary>
        /// 巨龙
        /// </summary>
        const string 高达 = "3";

        /// <summary>
        /// 泰坦
        /// </summary>
        const string 飞艇 = "4";

        /// <summary>
        /// 剑虎
        /// </summary>
        const string 加特林 = "5";

        /// <summary>
        /// 冰熊
        /// </summary>
        const string 坦克 = "6";

        /// <summary>
        /// 恶狼
        /// </summary>
        const string 机关枪 = "7";

        /// <summary>
        /// 火犬
        /// </summary>
        const string 冰冻塔 = "8";

        readonly string[] numbers = new[]
        {
            喷火器,加特林,冰冻塔,激光塔,高达,坦克,机关枪,飞艇
        };

        #endregion

        protected override Odds DefaultOdds => new Dictionary<string, decimal>()
        {
            {喷火器,5 },
            {加特林,10 },
            {冰冻塔,5 },
            {激光塔,5 },
            {高达,35 },
            {坦克,10 },
            {机关枪,5 },
            {飞艇,25 }
        };

        protected override bool CheckBetContent(string content)
        {
            return this.numbers.Contains(content);
        }

        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            return this.numbers.Contains((string)openNumber);
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            if(input != openNumber) return decimal.Zero;
            return odds.GetOdds(input);
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }
    }
}
