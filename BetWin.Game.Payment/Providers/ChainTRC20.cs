using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Exceptions;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    [Description("TRC20")]
    public sealed class ChainTRC20 : PaymentProviderBase
    {
        public string Gateway { get; set; } = "https://api.betwin.vip/";

        public string address { get; set; }

        public ChainTRC20(string setting) : base(setting)
        {
        }

        public override PaymentResponse Payment(PaymentRequest request)
        {
            if (this.handler == null) throw new PaymentException(PaymentError.NoPaymentHandler);
            //#1 汇率转换
            decimal? amount = this.handler.Conversion(request.amount, request.currency, PaymentCurrency.USDT);
            if (amount == null) throw new PaymentException(PaymentError.NoPaymentHandler);

            //#1 金额加上随机数
            amount += new Random().Next(100) / 10000M;
            request.amount = amount.Value;

            //#2 写入订单（同时判断金额是否被锁定）
            if (!this.handler.LockChainOrder(this.address, request)) throw new PaymentException(PaymentError.AmountLocked);

            //#3 返回数据内容
            return new PaymentResponse()
            {
                account = this.address,
                expire = WebAgent.GetTimestamps(DateTime.Now.AddMinutes(30)),
                amount = request.amount,
                currency = request.currency,
                orderId = request.orderId,
                url = $"{this.Gateway}?orderId={request.orderId}"
            };
        }
    }
}
