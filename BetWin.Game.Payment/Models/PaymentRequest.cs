using BetWin.Game.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 支付接口的请求内容
    /// </summary>
    public sealed class PaymentRequest
    {
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 充值的币种
        /// </summary>
        public PaymentCurrency currency { get; set; }

        public string? clientIp { get; set; }

        public string? notifyUrl { get; set; }

        public string? returnUrl { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string? orderId { get; set; }

        /// <summary>
        /// 请求充值的用户名
        /// </summary>
        public string? username { get; set; }

        /// <summary>
        /// 扩展内容
        /// </summary>
        public Dictionary<string,string>? data { get; set; }
    }
}
