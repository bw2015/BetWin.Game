using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Providers;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;


namespace BetWin.Game.Test
{
    /// <summary>
    /// 单元测试工具
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("按任意键");
            Console.ReadKey();
            //new Tests().InMoneyTest();

            var response = new JIFuYun("{}")
            {
                gateway = "https://api.a8.to/jump/?http://115.126.121.25:6005",
                appId = "178",
                merchantId = "178",
                key = "fd646be7-c843-46b0-b3f1-d52d1e3df3ed",
                publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3ebGBsuQc+ONzsoe/TNj7U/oYDfy/hhxCRtmm40D3wj9vs+25fOpA+LGx3CGyoS5/0l5BWA8K8/zBvMnvZegcggcaMIJGjdg6DJRoj4YNdLRuPsGxrQVI4+QfbAYkXVVzlUveERvBqJXAZ7Vt8hK+8Cyj3XYMDC/vHXpxh/xNCUS8U4YFX4VdAB70fKLZolqG8APPNpmhjoivz7PFjYD/n0i0FwlROrWtC2jDJiZN0npN0IISvQ8zGbRfuMqpmN181oc08BonwLSu73ZwMzDGXpaBiPdSWQSOFn479oXkP5E2DZ2DC6weBHjz1i9B+7S8JA56hyC8v0xCrCOUNE3jQIDAQAB",
                ditchId = "2"
            }.Payment(new PaymentRequest()
            {
                orderId = Guid.NewGuid().ToString("N"),
                amount = 1000,
                clientIp = "127.0.0.1",
                currency = PaymentCurrency.CNY,
                username = "test",
                notifyUrl = "https://www.baidu.com",
                returnUrl = "https://www.baidu.com",
            });

            Console.WriteLine(JsonConvert.SerializeObject(response));
        }

    }
}