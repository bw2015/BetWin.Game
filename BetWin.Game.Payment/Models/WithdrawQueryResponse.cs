using BetWin.Game.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 提现订单的查询返回
    /// </summary>
    public class WithdrawQueryResponse
    {
        /// <summary>
        /// 商户订单
        /// </summary>
        public string? OrderID { get; set; }

        /// <summary>
        /// 三方的付款流水号
        /// </summary>
        public string? TradeNo { get; set; }

        /// <summary>
        /// 付款的币种
        /// </summary>
        public PaymentCurrency? Currency { get; set; }

        /// <summary>
        /// 实际支付的金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 查询到的状态（为null表示发生异常）
        /// </summary>
        public WithdrawProviderStatus? Status { get; set; }
    }
}
