using BetWin.Game.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 提现请求的返回内容
    /// </summary>
    public class WithdrawResponse
    {
        /// <summary>
        /// 本地的订单号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 网关下发的交易流水号
        /// </summary>
        public string tradeNo { get; set; }

        /// <summary>
        /// 打款的状态（如果为null，则表示发生网络异常错误，没有获取到状态）
        /// </summary>
        public WithdrawProviderStatus? status { get; set; }

        /// <summary>
        /// 打款的币种
        /// </summary>
        public PaymentCurrency currency { get; set; }

        /// <summary>
        /// 实际打款的金额
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string msg { get; set; }
    }
}
