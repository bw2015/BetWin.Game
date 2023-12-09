using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Utils;
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
        public PaymentResponse()
        {
            this.createTime = WebAgent.GetTimestamps();
        }

        /// <summary>
        /// 本地的充值订单号
        /// </summary>
        public string? orderId { get; set; }

        public PaymentCurrency currency { get; set; }

        public decimal amount { get;  set; }

        public string? url { get;  set; }

        /// <summary>
        /// 收款账号（虚拟币或者银行卡收款）
        /// </summary>
        public string? account { get; set; }

        /// <summary>
        /// 支付订单的创建时间
        /// </summary>
        public long createTime { get; set; }

        /// <summary>
        /// 本次支付的有效时间（时间戳）
        /// </summary>
        public long? expire { get; set; }

        /// <summary>
        /// 需要传递的信息(一般用于错误信息的传递）
        /// </summary>
        public string? msg { get; set; }

        /// <summary>
        /// 前台要弹出的信息框
        /// </summary>
        public string? message { get; set; }

        /// <summary>
        /// 当前的供应商类型
        /// </summary>
        public string? provider { get; set; }

        public static implicit operator bool(PaymentResponse response)
        {
            return !string.IsNullOrEmpty(response?.url);
        }
    }
}
