using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    /// <summary>
    /// 官方采集 拯救小红帽
    /// </summary>
    [Description("快手小红帽")]
    internal class KSRed : CollectProviderBase
    {
        public string gateway { get; set; } = "https://rescue.cd-lucian.com/api/v1/game/game/currentGame";

        public string token { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2NDg5YWRkOTI0OWU4YzRlZjYyMzhjMzUiLCJvcGVuSWQiOiJmMTk3NGU2ZTNlNTBmMTI4MTRiZjU3MjQ0ODFkNzNkOSIsImV4cCI6MTcwMTAwMDc5MCwiaWF0IjoxNzAwNzQxNTkwLCJpc3MiOiJsdWNpYW4ifQ.wirGS5txEF9gw8ptIfoqqCkyy_dpPXJqTN1CV6H0maw";


        public KSRed(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3);
                HttpClientResponse result = client.Post(this.gateway, @"{""token"":""""}", new Dictionary<string, string>()
                {
                    {"Content-Type","application/json" },
                    {"projectid","rescue_girl" },
                    {"x-client-info","model=iPhone11,6;os=iOS;nqe-score=28;network=WIFI;" },
                    {"Referer","https://miniapi.ksapisrv.com/ks709879895501315735/48969/page-frame.html" },
                    {"user-agent","Mozilla/5.0 (iPhone; CPU iPhone OS 17_1_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 NetType/wifi Language/zh-Hans-CN;q=1, en-CN;q=0.9, zh-Hant-CN;q=0.8 kwapp/1.1.300.0 miniProgram/1.64.0 KUAISHOU/11.10.20.8536" },
                    {"token",this.token }
                });
                if (!result) yield break;

                response? res = JsonConvert.DeserializeObject<response>(result);
                if (res == null || res.code != 0) yield break;

                gameInfo? game = res?.data?.gameInfo;
                if (game == null) yield break;
                string index;
                switch (game.status)
                {
                    case 1:
                        // 当前的可投注期
                        index = this.getOpenTime(game.openTime).ToString("yyyyMMddHHmmss");
                        this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(index, WebAgent.GetTimestamps(game.openTime), WebAgent.GetTimestamps(game.startTime), WebAgent.GetTimestamps(game.betTime)));

                        // 上一期的开奖结果
                        DateTime openTime = game.openTime.AddSeconds(-55);
                        index = this.getOpenTime(openTime).ToString("yyyyMMddHHmmss");
                        yield return new CollectData(index, this.getNumber(game.result), WebAgent.GetTimestamps(openTime));

                        break;
                    case 3:
                        index = this.getOpenTime(game.openTime).ToString("yyyyMMddHHmmss");
                        yield return new CollectData(index, this.getNumber(game.result), WebAgent.GetTimestamps(game.openTime));
                        break;
                }
            }
        }

        string getNumber(int number)
        {
            return number switch
            {
                3 => "5",
                5 => "3",
                _ => number.ToString()
            };
        }

        DateTime getOpenTime(DateTime openTime)
        {
            int second = openTime.Second;
            openTime = openTime.AddSeconds(-second);
            second = (int)(Math.Round(second / 5D) * 5);
            return openTime.AddSeconds(second);
        }

        class response
        {
            public int code { get; set; }

            public long now { get; set; }

            public int cmdId { get; set; }

            public responseData data { get; set; }

            public string msg { get; set; }
        }

        class responseData
        {
            public DateTime serverTime { get; set; }

            public gameInfo? gameInfo { get; set; }
        }

        class gameInfo
        {
            public int result { get; set; }

            public int status { get; set; }

            public string recordId { get; set; }

            public DateTime startTime { get; set; }

            public DateTime betTime { get; set; }

            public DateTime openTime { get; set; }

            public DateTime rewardTime { get; set; }

            public DateTime endTime { get; set; }

            public int baseScale { get; set; }
        }
    }
}
