using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("快手-怪物捕手")]
    internal class KSgwbs : CollectProviderBase
    {
        public string gateway { get; set; } = "https://gwbs-ks.sskjz.com/api/v1/status";

        /// <summary>
        /// 获取局号信息
        /// </summary>
        public string round { get; set; } = "https://gwbs-ks.sskjz.com/api/v1/status/round";

        public string token { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvcGVuSWQiOiJmMTgzODNkNjYyODA3MTRmMTRiZjU3MjQ0ODFkNzNkOSIsInNlc3Npb25JZCI6ImNjMjMwNzkyLThhZjgtMTFlZS1iYjYzLTAwMTYzZTBjNGMyMCIsInYiOiIxLjAuMCIsImV4cCI6MTcwMTQ1NTk2NywiaXNzIjoic3AifQ.phlZIeXeGYd7uOO96fcA9_kmp1Uqm4W-3N_-jlO2BME";

        public KSgwbs(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                 {"content-type","application/json" },
                    {"x-app-roomid","sUB5VTQei3U" },
                    {"x-app-anchoropenid","f18383d66280714f14bf572418a5ab4a" },
                    {"x-app-ct",DateTime.Now.ToString("yyyyMMdd HH:mm:ss:FFF") },
                    {"user-agent","com_kwai_gif/11.10.20 (iPhone; iOS 17.1.1; Scale/3.00)" },
                    {"x-app-version","3.1.1" },
                    {"x-access-token",this.token }
            };
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3);

                HttpClientResponse result = client.Get(this.gateway, headers);
                if (!result) yield break;

                response? res = JsonConvert.DeserializeObject<response>(result);
                if (res == null || res.code != 0) yield break;

                if (res.data == null || res.data.monsterIds == null || !res.data.monsterIds.Any()) yield break;

                yield return new CollectData(res.data.roundId.ToString(), this.getNumber(res.data.monsterIds.First()), res.data.roundId * 1000L);

                string nextIndex = (res.data.roundId + 50).ToString();

                this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(nextIndex, (res.data.roundId + 50) * 1000L, res.data.roundId * 1000L));
            }
        }

        /// <summary>
        /// 转换号码
        /// </summary>
        /// <param name="monsterId"></param>
        /// <returns></returns>
        string getNumber(int monsterId)
        {
            return monsterId switch
            {
                3 => "7",
                4 => "8",
                5 => "6",
                6 => "5",
                7 => "4",
                8 => "3",
                _ => monsterId.ToString()
            };
        }

        class response
        {
            public int code { get; set; }

            public responseData? data { get; set; }
        }

        class responseData
        {
            /// <summary>
            /// 局号（11位时间戳）
            /// </summary>
            public long roundId { get; set; }

            /// <summary>
            /// 开奖号码
            /// </summary>
            public int[]? monsterIds { get; set; }
        }
    }
}
