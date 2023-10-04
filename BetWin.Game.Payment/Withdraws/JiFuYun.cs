using BetWin.Game.Payment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Withdraws
{
    [Description("极付云")]
    public class JiFuYun : WithdrawProviderBase
    {
        [Description("网关")]
        public string gateway { get; set; } = "http://115.126.121.25:6005";

        [Description("appId")]
        public string appId { get; set; } = "178";

        [Description("key")]
        public string key { get; set; }

        [Description("公钥")]
        public string publicKey { get; set; }

        [Description("商户ID")]
        public string merchantId { get; set; }

        /// <summary>
        /// 1支付宝转卡，2银行卡转卡，3微信转卡
        /// </summary>
        [Description("渠道编号")]
        public string ditchId { get; set; }

        /// <summary>
        /// 1 支付宝转支付宝
        /// 2 支付宝转银行卡
        /// 3 银行卡转银行卡
        /// 4 微信转银行卡
        /// 6 数字人民币
        /// 7 数字人民币转银行卡
        /// </summary>
        [Description("支付类型")]
        public string usePayType { get; set; }

        public JiFuYun(string setting) : base(setting)
        {
        }

        public override WithdrawResponse Withdraw(WithdrawRequest request)
        {
            string url = $"{this.gateway}/api/Order/OutMoneyOrder";

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"identifier",request.orderId ?? string.Empty },
                {"sum", request.amount * 100 },
                {"merchantId",this.merchantId },
                {"ditchId",this.ditchId },
                {"payee",request.bankName ?? string.Empty },
                {"account",request.account ?? string.Empty },
                {"city",string.Empty },
                {"bankName",request.bankCode.ToString() },
                {"userId",request.userName ?? string.Empty },
                {"userlevel",0 },
                {"usePayType",this.usePayType }
            };

            throw new NotImplementedException();
        }

        public override WithdrawResponse Query(WithdrawRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
