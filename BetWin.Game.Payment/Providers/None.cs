using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Microsoft.AspNetCore.Http;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Providers
{

    [Description("代客充值")]
    public sealed class None : PaymentProviderBase
    {
        public None(string setting) : base(setting)
        {
        }

        protected override CallbackResponse callback(HttpContext context)
        {
            throw new NotImplementedException();
        }

        protected override PaymentResponse payment(PaymentRequest request, out string url, out string postData, out HttpClientResponse result)
        {
            postData = string.Empty;
            result = default;
            url = string.Empty;
            return new PaymentResponse
            {
                createTime = WebAgent.GetTimestamps(),
                amount = request.amount,
                currency = request.currency,
                orderId = request.orderId
            };
        }
    }
}
