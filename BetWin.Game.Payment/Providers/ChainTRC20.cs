using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Exceptions;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    [Description("TRC20")]
    public sealed class ChainTRC20 : PaymentProviderBase
    {
        /// <summary>
        /// 超时时间（分钟）
        /// </summary>
        public const int EXPIRE_MINUTE = 30;

        public string Gateway { get; set; } = "https://pay.betwin.vip/";

        public string address { get; set; }

        public ChainTRC20(string setting) : base(setting)
        {
        }

        protected override CallbackResponse callback(HttpContext context)
        {
            return default;
        }

        protected override PaymentResponse payment(PaymentRequest request, out string postData, out HttpClientResponse result)
        {
            postData = string.Empty;
            result = default;
            if (this.handler == null) throw new PaymentException(PaymentError.NoPaymentHandler);
            //#1 汇率转换
            decimal? amount = this.handler.Conversion(request.amount, request.currency, PaymentCurrency.USDT);
            if (amount == null) throw new PaymentException(PaymentError.NoPaymentHandler);

            //#1 金额加上随机数
            amount += new Random().Next(100) / 10000M;

            PaymentResponse response = new PaymentResponse()
            {
                account = this.address,
                expire = WebAgent.GetTimestamps(DateTime.Now.AddMinutes(EXPIRE_MINUTE)),
                amount = amount.Value,
                currency = request.currency,
                orderId = request.orderId,
                url = $"{this.Gateway}?orderId={request.orderId}"
            };

            //#2 写入订单（同时判断金额是否被锁定）
            if (!this.handler.SaveChainOrder(response)) throw new PaymentException(PaymentError.AmountLocked);

            //#3 返回数据内容
            return response;
        }
    }
}
