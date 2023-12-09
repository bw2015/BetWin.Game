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
    /// <summary>
    /// 银行转账
    /// </summary>
    [Description("银行转账")]
    internal class BankTransfer : PaymentProviderBase
    {
        [Description("开户银行")]
        public string Bank { get; set; }

        [Description("收款人")]
        public string Name { get; set; }

        [Description("账号")]
        public string Account { get; set; }

        public BankTransfer(string setting) : base(setting)
        {
        }

        protected override CallbackResponse callback(HttpContext context)
        {
            throw new NotImplementedException();
        }

        protected override PaymentResponse payment(PaymentRequest request, out string url, out string postData, out HttpClientResponse result)
        {
            postData = new { this.Bank, this.Name, this.Account }.ToJson();
            result = new HttpClientResponse();
            url = new { Bank, Name, Account }.ToJson();
            return new PaymentResponse()
            {
                provider = this.GetType().Name,
                url = url,
                orderId = request.orderId,
                amount = request.amount
            };
        }
    }
}
