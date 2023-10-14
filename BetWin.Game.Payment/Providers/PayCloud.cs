using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace BetWin.Game.Payment.Providers
{
    [Description("极付云")]
    public class PayCloud : PaymentProviderBase
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

        public PayCloud(string setting) : base(setting)
        {
        }

        private static HttpClient client = new HttpClient();

        /// <summary>
        /// 回调信息
        /// </summary>
        protected override CallbackResponse callback(HttpContext context)
        {
            string body = context.GetString();
            if (string.IsNullOrEmpty(body))
            {
                return default;
            }
            JObject info = JObject.Parse(body);
            if (info["state"]?.Value<int>() != 1)
            {
                return new CallbackResponse()
                {
                    result = "false"
                };
            };

            string netPublicKey = new RsaHelper().RSAPublicKeyJava2DotNet(this.publicKey);
            RsaHelper rsaHelper = new RsaHelper(netPublicKey, null, false);
            JObject result = JObject.Parse(rsaHelper.Decrypt(info["text"]?.Value<string>() ?? string.Empty));

            return new CallbackResponse()
            {
                success = true,
                status = HttpStatusCode.OK,
                orderId = info["identifier"]?.Value<string>(),
                tradeNo = info["identifier"]?.Value<string>(),
                currency = PaymentCurrency.CNY,
                amount = result["orderInfo"]?["payeeSum"]?.Value<decimal>() / 100M,
                result = "true"
            };
        }

        protected override PaymentResponse payment(PaymentRequest request, out string url, out string postData, out HttpClientResponse result)
        {
            string netPublicKey = new RsaHelper().RSAPublicKeyJava2DotNet(this.publicKey);
            RsaHelper rsaHelper = new RsaHelper(netPublicKey, null, false);

            url = $"{this.gateway}/api/Order/InMoneyOrder";

            Dictionary<string, string> pairs = new Dictionary<string, string>()
            {
                {"appId",this.appId},
                {"timestamp",(WebAgent.GetTimestamps()/1000L).ToString()},
                {"type","1" },
                {"state","1" },
                {"identifier",request.orderId?.ToString() ?? string.Empty },
                {"merchantId",this.merchantId },
                {
                    "text",
                    rsaHelper.Encrypt(JsonConvert.SerializeObject(new
                    {
                        identifier = request.orderId?.ToString() ?? string.Empty,
                        sonNumber = string.Empty,
                        sum = (int)(request.amount * 100M),
                        this.merchantId,
                        this.ditchId,
                        name = nameof(PayCloud),
                        userId = request.username,
                        userlevel = 6,
                        usePayType = 3,
                        customUrl = request.notifyUrl,
                        orderFlag = string.Empty,
                        universalparameters = string.Empty,
                        transparentValue = string.Empty
                    }))
                }
            };


            Dictionary<string, string> md5Pairs = pairs.OrderBy(c => c.Key).ToDictionary(x => x.Key, x => x.Value);
            md5Pairs.Add("apikey", this.key);
            //签名第二步：把字典数据拼接成字符串
            string signData = string.Join("&", md5Pairs.Select(t => $"{t.Key}={WebUtility.UrlEncode(t.Value)}"));
            //签名第三步：获取签名
            string sign = rsaHelper.md5Signature(signData);
            md5Pairs.Add("sign", sign);
            md5Pairs.Remove("apikey");

            postData = string.Join("&", md5Pairs.Select(t => $"{t.Key}={WebUtility.UrlEncode(t.Value)}"));

            result = client.Post(url, postData, new Dictionary<string, string>()
            {
                {"Content-Type","application/x-www-form-urlencoded" }
            });

            JObject info = JObject.Parse(result);
            if (info["status"]?.Value<int>() != 200)
            {
                return new PaymentResponse()
                {
                    msg = info["msg"]?.Value<string>()
                };
            }

            if (info["data"]?["state"]?.Value<int>() != 1)
            {
                return new PaymentResponse()
                {
                    msg = info["data"]?["errorText"]?.Value<string>()
                };
            }

            string text = info["data"]?["text"]?.Value<string>() ?? string.Empty;
            JObject textInfo = JObject.Parse(rsaHelper.Decrypt(text));


            return new PaymentResponse()
            {
                orderId = info["orderId"]?.Value<string>(),
                amount = info["sum"]?.Value<int>() / 100M ?? 0,
                createTime = WebAgent.GetTimestamps(info["addDate"]?.Value<DateTime>() ?? DateTime.Now),
                currency = PaymentCurrency.CNY,
                url = textInfo["payUrl"]?.Value<string>(),
                account = textInfo["account"]?.Value<string>()
            };
        }
    }
}
