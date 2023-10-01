using BetWin.Game.Payment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    public interface IPaymentProvider
    {
        PaymentResponse Payment(PaymentRequest request);

        /// <summary>
        /// 支持回调
        /// </summary>
        CallbackResponse Callback(HttpContext context);
    }
}
