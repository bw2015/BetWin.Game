using BetWin.Game.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 请求支付接口的返回内容
    /// </summary>
    public sealed class PaymentResponse
    {
        /// <summary>
        /// 本地的充值订单号
        /// </summary>
        public string? orderId { get; internal set; }

        public PaymentCurrency currency { get; internal set; }

        public decimal amount { get; internal set; }

        public string? url { get; internal set; }

        /// <summary>
        /// 收款账号（虚拟币或者银行卡收款）
        /// </summary>
        public string? account { get; internal set; }

        /// <summary>
        /// 本次支付的有效时间（时间戳）
        /// </summary>
        public long? expire { get; internal set; }

        public static implicit operator bool(PaymentResponse response)
        {
            return !string.IsNullOrEmpty(response.url);
        }
    }
}
