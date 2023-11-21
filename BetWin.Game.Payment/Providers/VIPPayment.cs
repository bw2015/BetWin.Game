using BetWin.Game.Payment.Models;
using Microsoft.AspNetCore.Http;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    /// <summary>
    /// 联系客服的代客充值
    /// </summary>
    [Description("代客充值")]
    internal class VIPPayment : PaymentProviderBase
    {
        /// <summary>
        /// 需要弹出的提示框
        /// </summary>
        [Description("提示信息")]
        public string message { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        [Description("客服连接")]
        public string url { get; set; }

        public VIPPayment(string setting) : base(setting)
        {
        }

        protected override CallbackResponse callback(HttpContext context)
        {
            throw new NotImplementedException();
        }

        protected override PaymentResponse payment(PaymentRequest request, out string url, out string postData, out HttpClientResponse result)
        {
            postData = string.Empty;
            result = new HttpClientResponse();
            url = string.Empty;
            return new PaymentResponse()
            {
                orderId = request.orderId,
                url = this.url,
                amount = request.amount,
                message = message
            };
        }
    }
}
