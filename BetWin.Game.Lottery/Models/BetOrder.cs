using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Models
{

    public struct BetOrderResult
    {
        /// <summary>
        /// 统计周期内的总投注金额
        /// </summary>
        public decimal BetMoney { get; set; }

        /// <summary>
        /// 统计周期内的总盈亏
        /// </summary>
        public decimal Money { get; set; }

        public List<BetOrder> Orders { get; set; }
    }

    /// <summary>
    /// 投注订单
    /// </summary>
    public struct BetOrder
    {
        public BetOrder(string play, string betContent, decimal betMoney)
        {
            this.Play = play;
            this.BetContent = betContent;
            this.BetMoney = betMoney;
        }

        /// <summary>
        /// 玩法
        /// </summary>
        public string Play;

        /// <summary>
        /// 投注的内容
        /// </summary>
        public string BetContent;

        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal BetMoney;
    }
}
