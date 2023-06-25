using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Models
{
    /// <summary>
    /// 开奖所需的计算条件
    /// </summary>
    public class OpenResultModel
    {
        /// <summary>
        /// 杀率 0 = 无杀率  1 = 包杀
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 本期的投注内容
        /// </summary>
        public IEnumerable<OpenResultOrder>? Orders { get; set; }

        /// <summary>
        /// 计算周期内的投注金额
        /// </summary>
        public decimal BetMoney { get; set; }

        /// <summary>
        /// 计算周期内的总派奖金额
        /// </summary>
        public decimal Reward { get; set; }
    }

    public struct OpenResultOrder
    {
        /// <summary>
        /// 玩法
        /// </summary>
        public string PlayType { get; set; }

        /// <summary>
        /// 投注金额（多倍投注在外部计算好金额）
        /// </summary>
        public decimal BetMoney { get; set; }

        /// <summary>
        /// 投注内容
        /// </summary>
        public string Content { get; set; }
    }
}
