using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 查询提现订单对象
    /// </summary>
    public class WithdrawQuery
    {
        /// <summary>
        /// 商户的订单号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 上游的流水单号
        /// </summary>
        public string tradeNo { get; set; }
    }
}
