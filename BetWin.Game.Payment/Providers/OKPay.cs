using BetWin.Game.Payment.Models;
using Microsoft.AspNetCore.Http;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    public class OKPay : PaymentProviderBase
    {
        public OKPay(string setting) : base(setting)
        {
        }

        protected override CallbackResponse callback(HttpContext context)
        {
            throw new NotImplementedException();
        }

        protected override PaymentResponse payment(PaymentRequest request, out string url, out string postData, out HttpClientResponse result)
        {
            throw new NotImplementedException();
        }
    }
}
