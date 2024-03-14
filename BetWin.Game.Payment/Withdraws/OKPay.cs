using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Payment.Withdraws
{
    [Description("OKPay")]
    public class OKPay : WithdrawProviderBase
    {
        [Description("提现网关")]
        public string Gateway { get; set; } = "";

        [Description("查询地址")]
        public string QueryUrl { get; set; } = "";

        [Description("商户编号")]
        public string sendid { get; set; }

        [Description("密钥")]
        public string apikey { get; set; }

        public OKPay(string setting) : base(setting)
        {
        }

        public override WithdrawQueryResponse Query(WithdrawQuery request)
        {
            string url = $"{QueryUrl}?id={request.tradeNo}";
            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(url, new Dictionary<string, string>());
                JObject info = JObject.Parse(result.Content);

                int? code = info["code"]?.Value<int>();
                string? msg = info["msg"]?.Value<string>();
                string? resultData = info["data"]?.Value<string>();
                Transaction? transaction = resultData?.ToJson<Transaction>();

                if (code == null || string.IsNullOrEmpty(resultData) || transaction == null)
                {
                    return new WithdrawQueryResponse()
                    {
                        Status = WithdrawProviderStatus.Unknow
                    };
                }

                return new WithdrawQueryResponse()
                {
                    Amount = transaction.amount,
                    Currency = PaymentCurrency.CNY,
                    OrderID = request.orderId,
                    TradeNo = transaction.id,
                    Status = transaction.state switch
                    {
                        4 => WithdrawProviderStatus.Success,
                        _ => WithdrawProviderStatus.Unknow
                    }
                };
            }
        }

        public override WithdrawResponse Withdraw(WithdrawRequest request)
        {
            string data = new
            {
                sendid,
                orderid = request.orderId,
                amount = request.amount.ToString("0.00"),
                address = request.account,
                sign = this.getSign(request.orderId, request.amount),
            }.ToJson();

            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Post(this.Gateway, data, new Dictionary<string, string>()
                {
                    {"Content-Type","application/json" }
                });

                JObject info = JObject.Parse(result.Content);

                int? code = info["code"]?.Value<int>();
                string? msg = info["msg"]?.Value<string>();
                string? resultData = info["data"]?.Value<string>();

                if (code != 1)
                {
                    return new WithdrawResponse()
                    {
                        status = WithdrawProviderStatus.Faild,
                        amount = request.amount,
                        currency = PaymentCurrency.CNY,
                        msg = msg ?? string.Empty,
                        orderId = request.orderId,
                        tradeNo = string.Empty
                    };
                }

                if (string.IsNullOrEmpty(resultData))
                {
                    return new WithdrawResponse()
                    {
                        status = WithdrawProviderStatus.Unknow,
                        amount = 0,
                        currency = PaymentCurrency.CNY,
                        orderId = request.orderId,
                        tradeNo = string.Empty,
                        msg = result.Content
                    };
                }

                Transaction? transaction = resultData.ToJson<Transaction>();
                if (transaction == null)
                {
                    return new WithdrawResponse()
                    {
                        status = WithdrawProviderStatus.Unknow,
                        amount = 0,
                        currency = PaymentCurrency.CNY,
                        orderId = request.orderId,
                        tradeNo = string.Empty,
                        msg = result.Content
                    };
                }

                return new WithdrawResponse()
                {
                    status = WithdrawProviderStatus.Success,
                    amount = transaction.amount,
                    currency = PaymentCurrency.CNY,
                    orderId = request.orderId,
                    tradeNo = transaction.id,
                    msg = msg ?? string.Empty
                };
            }
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        private string getSign(string orderId, decimal amount)
        {
            string signStr = string.Concat(this.sendid, orderId, amount.ToString("0.00"), this.apikey);
            return signStr.toMD5().ToLower();
        }

        #region ======== 实体类  ========

        public class Transaction
        {
            /// <summary>
            /// 金额
            /// </summary>
            public decimal amount { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public decimal amountDec { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime createtime { get; set; }

            /// <summary>
            /// 交易ID
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string note { get; set; }

            /// <summary>
            /// 订单ID
            /// </summary>
            public string orderid { get; set; }

            /// <summary>
            /// 订单类型
            /// </summary>
            public string ordertype { get; set; }

            /// <summary>
            /// 接收费用
            /// </summary>
            public string recvcharge { get; set; }

            /// <summary>
            /// 接收ID
            /// </summary>
            public string recvid { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string remark { get; set; }

            /// <summary>
            /// 手续费
            /// </summary>
            public decimal sendcharge { get; set; }

            /// <summary>
            /// 发送ID
            /// </summary>
            public string sendid { get; set; }

            /// <summary>
            /// 签名
            /// </summary>
            public string sign { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            public int state { get; set; }

            /// <summary>
            /// 交易时间
            /// </summary>
            public DateTime? transtime { get; set; }
        }


        #endregion
    }
}
