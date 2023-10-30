using BetWin.Game.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 回调的返回内容
    /// </summary>
    public struct CallbackResponse
    {
        /// <summary>
        /// 错误的输出代码
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        public CallbackResponse(HttpStatusCode status, string result) : this()
        {
            this.status = status;
            this.result = result;
        }

        /// <summary>
        /// 返回的状态码
        /// </summary>
        public HttpStatusCode status { get; set; }

        /// <summary>
        /// 返回内容
        /// </summary>
        public string result { get; set; }

        /// <summary>
        /// 是否支付成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public PaymentCurrency? currency { get; set; }

        /// <summary>
        /// 实际支付的金额
        /// </summary>
        public decimal? amount { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string? orderId { get; set; }

        /// <summary>
        /// 上游的订单号
        /// </summary>
        public string? tradeNo { get; set; }

    }
}
