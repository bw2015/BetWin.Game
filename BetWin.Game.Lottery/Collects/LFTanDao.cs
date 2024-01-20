using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace BetWin.Game.Lottery.Collects
{
    [Description("探岛寻宝")]
    public class LFTanDao : CollectProviderBase
    {
        public LFTanDao(string setting) : base(setting)
        {
        }

        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "LF.Land";

        public string appKey { get; set; } = "24679788";

        public override LotteryType Type => LotteryType.Smart;

        private static long lastTime = 0;

        public override IEnumerable<CollectData> Execute()
        {
            if (lastTime + 60 * 1000 > WebAgent.GetTimestamps()) yield break;
            using (HttpClient client = new HttpClient())
            {
                string url = $"{gateway}?key={this.key}";
                string content = client.Get(url, new Dictionary<string, string>());
                JObject config = JObject.Parse(content);
                string? link = config?["gateway"]?.Value<string>();
                string? cookie = config?["cookie"]?.Value<string>();
                if (link == null || cookie == null) yield break;

                string postData = "data={\"jsonParam\":\"{\\\"roomId\\\":0,\\\"mrpStatus\\\":1}\",\"ename\":\"tandaoxunbao1\",\"pageNo\":1,\"pageSize\":20}";
                var result = client.Post(link, postData, new Dictionary<string, string>()
                {
                    {"Content-Type","application/x-www-form-urlencoded" },
                    {"accept","application/json" },
                    {"sec-fetch-site","same-site" },
                    {"accept-encoding","gzip, deflate" },
                    {"accept-language","en-US" },
                    {"sec-fetch-mode","cors" },
                    {"origin","https://zone.laifeng.com" },
                    {"user-agent","Mozilla/5.0 (iPhone; CPU iPhone OS 17_2_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148laifeng_ios_9.0.8_533 AliApp(Laifeng/9.0.8) WindVane/8.8.0-noBTCTLO 1242x2688" },
                    {"referer","https://zone.laifeng.com/" },
                    {"sec-fetch-dest","empty" },
                    {"Cookie",cookie }
                });

                response? res = result.Content.ToJson<response>();
                if (res?.data?.dateList == null) yield break;

                foreach (var item in res.data.dateList)
                {
                    long openTime = item.scene + 50 * 1000;
                    yield return new CollectData((item.scene / 10000L).ToString(), this.getNumber(item.rewardName), openTime);
                    if (lastTime < openTime) lastTime = openTime;
                }
            }
        }

        string getNumber(string name)
        {
            return name switch
            {
                "紫烟岛" => "1",
                "银月岛" => "2",
                "梦境岛" => "3",
                "绿洲岛" => "4",
                "凤舞岛" => "5",
                "黑石岛" => "6",
                "龙鳞岛" => "7",
                "蓝海岛" => "8",
                _ => "0"
            };
        }

        class response
        {
            public string api { get; set; }

            public responseData data { get; set; }
        }

        class responseData
        {
            public responseItem[] dateList { get; set; }
        }

        class responseItem
        {
            public int mrpStatus { get; set; }

            /// <summary>
            /// 名字
            /// </summary>
            public string rewardName { get; set; }

            /// <summary>
            /// 期号
            /// </summary>
            public long scene { get; set; }
        }
    }
}
