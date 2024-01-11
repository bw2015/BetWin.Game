using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("抓鸭子")]
    public class SKRDuck : CollectProviderBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string gateway { get; set; } = "https://act.inframe.mobi/v1/mstask/catch-duck/lottery-record?offset=0&cnt=20&limit=20";

        public string cookie { get; set; } = "S=lt%3D1704778651269260703%26ut%3D1704778651269260703%26s%3D11928da99c5c392d74b08477d147bc15;T=i%3D15594366%26u%3D%26n%3DU2tyMTU1OTQ%3D%26e%3D%26p%3DMTg2ODg4NTg1Nzg%3D%26a%3DaHR0cDovL3Jlcy1zdGF0aWMuaW5mcmFtZS5tb2JpL3VpLzE2NzI4MTIyNDM5NTQzNy5wbmc%3D%26ct%3D1704778651";

        public SKRDuck(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        private static long lastTime = 0;

        public override IEnumerable<CollectData> Execute()
        {
            List<CollectData> list = new List<CollectData>();
            long now = WebAgent.GetTimestamps() / 1000L;
            if (now - lastTime < 30) return list;

            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(this.gateway, new Dictionary<string, string>()
                {
                    {"accept","application/json, text/plain, */*" },
                    {"sec-fetch-site","same-origin" },
                    {"inframe-client-id","39E53030" },
                    {"accept-encoding","gzip, deflate, br" },
                    {"sec-fetch-mode","cors" },
                    {"user-agent","Mozilla/5.0 (iPhone; CPU iPhone OS 17_1_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 pid/39E53030-2721-4381-B06B-2F420EBED052 NetType/WiFi Pixel/1242 appVersion/5008003 DeviceName/iPhone XS Max CN" },
                    {"referer","https://act.inframe.mobi/ducks/?roomID=2501056&title=1&userID=15594366&cache=1" },
                    {"sec-fetch-dest","empty" },
                    {"cookie",this.cookie }
                });

                if (!result) return list;

                response? response = result.Content.ToJson<response>();

                if (response?.data?.items == null) return list;

                foreach (var item in response.data.items)
                {
                    list.Add(new CollectData(item.gameID.ToString(), this.getNumber(item.duckName), item.createdTime * 1000L));
                }

                lastTime = response.data.items.Max(t => t.createdTime);
                int gameId = response.data.items.Max(t => t.gameID);

                this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel((gameId + 1).ToString(), (lastTime + 30) * 1000L, lastTime * 1000L));

                return list;
            }
        }

        private string getNumber(string? name)
        {
            return name switch
            {
                "厉害鸭" => "1",   // 1.6%
                "加油鸭" => "2",   // 3%
                "心动鸭" => "3",   // 7%
                "唱歌鸭" => "4",   // 8%
                "划水鸭" => "5",   // 21%
                "拴Q鸭" => "6",   // 17.5% 
                "干饭鸭" => "7",   // 10.6%
                "吃瓜鸭" => "8",   // 31.3%
                _ => "0"
            };
        }

        class response
        {
            public int errno { get; set; }

            public string? errmsg { get; set; }

            public responseData? data { get; set; }
        }

        class responseData
        {
            public bool hasMore { get; set; }

            public int offset { get; set; }

            public responseItem[]? items { get; set; }
        }

        class responseItem
        {
            public int gameID { get; set; }

            /// <summary>
            /// 开奖时间（秒）
            /// </summary>
            public long createdTime { get; set; }

            public string? duckUrl { get; set; }

            /// <summary>
            /// 鸭子名字
            /// </summary>
            public string? duckName { get; set; }

            /// <summary>
            /// 开奖号码
            /// </summary>
            public int position { get; set; }
        }
    }
}
