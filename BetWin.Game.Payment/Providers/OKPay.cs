using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    public class OKPay : PaymentProviderBase
    {
        [Description("网关")]
        public string gateway { get; set; } = "https://dsa12rsdaa.okpaydj.com/createpay";

        [Description("商户ID")]
        public string recvid { get; set; } = "6504aa2d-e6c4-49ae-9853-dc210242f1d1";

        [Description("ApiKey")]
        public string ApiKey { get; set; } = "e039b201f81c48d3847b317777f2000d";

        public OKPay(string setting) : base(setting)
        {
        }

        protected override CallbackResponse callback(HttpContext context)
        {
            if (context.Request.Method == "GET")
            {
                return new CallbackResponse(HttpStatusCode.OK, "充值成功")
                {
                    success = false
                };
            }

            string content = context.GetString();
            JObject info = JObject.Parse(content);

            int? state = info["state"]?.Value<int>();
            if (state != 4)
            {
                return new CallbackResponse(HttpStatusCode.OK, state.ToString())
                {
                    success = false
                };
            }

            return new CallbackResponse()
            {
                amount = info["amount"]?.Value<decimal>(),
                currency = PaymentCurrency.CNY,
                orderId = info["orderid"]?.Value<string>(),
                result = state.ToString(),
                status = HttpStatusCode.OK,
                success = true,
                tradeNo = info["orderid"]?.Value<string>()
            };
        }

        protected override PaymentResponse payment(PaymentRequest request, out string url, out string postData, out HttpClientResponse result)
        {
            url = this.gateway;
            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                {"recvid",this.recvid},
                {"orderid", request.orderId ?? string.Empty },
                {"amount",request.amount.ToString("0.00") },
                {"notifyurl", request.notifyUrl ?? string.Empty},
                {"returnurl",request.returnUrl ?? string.Empty },
                {"note",request.username ?? string.Empty}
            };

            string signStr = string.Concat(dic["recvid"], dic["orderid"], dic["amount"], this.ApiKey);
            dic.Add("sign", signStr.toMD5().ToLower());
            postData = dic.ToJson();
            using (HttpClient client = new HttpClient())
            {
                result = client.Post(this.gateway, postData, new Dictionary<string, string>()
                {
                    {"Content-Type","application/json" }
                });

                if (!result)
                {
                    return new PaymentResponse()
                    {
                        msg = result.StatusCode.ToString()
                    };
                }

                JObject info = JObject.Parse(result);
                if (info["code"]?.Value<int>() != 1)
                {
                    return new PaymentResponse()
                    {
                        msg = info["msg"]?.Value<string>()
                    };
                }

                JObject data = JObject.Parse(info["data"]?.Value<string>() ?? "{}");
                return new PaymentResponse()
                {
                    amount = data["amount"]?.Value<decimal>() ?? request.amount,
                    account = data["qrurl"]?.Value<string>(),
                    createTime = WebAgent.GetTimestamps(data["createtime"]?.Value<DateTime>() ?? DateTime.Now),
                    currency = PaymentCurrency.CNY,
                    orderId = data["orderid"]?.Value<string>(),
                    url = data["navurl"]?.Value<string>()
                };
            }
        }
    }
}
