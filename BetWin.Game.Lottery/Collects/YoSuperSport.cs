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
    [Description("超能运动会")]
    internal class YoSuperSport : CollectProviderBase
    {
        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "YY.SuperSport";

        public YoSuperSport(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        private static HttpClient? _client;

        static HttpClient Client
        {
            get
            {
                return _client ??= new HttpClient();
            }
        }

        public override IEnumerable<CollectData> Execute()
        {
            string url = $"{this.gateway}?key={this.key}";
            string result = Client.Get(url, new Dictionary<string, string>());
            if (string.IsNullOrEmpty(result)) yield break;

            response[]? list = result.ToJson<response[]>();
            if (list == null || list.Length == 0) yield break;
            foreach (response res in list)
            {
                if (res.OpenTime == 0) yield break;
                yield return new CollectData(res.Index, res.openNumber.ToString(), res.OpenTime);
            }

            response newResult = list.FirstOrDefault();
            this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(
                 WebAgent.GetTimestamps(newResult.OpenTime + 40 * 1000).ToString("yyyyMMddHHmmss"),
                 newResult.OpenTime + 40 * 1000,
                 newResult.OpenTime,
                 newResult.OpenTime + 30 * 1000
                ));
        }


        class response
        {
            /// <summary>
            /// 开奖号码
            /// </summary>
            public int openNumber { get; set; }

            /// <summary>
            /// 开奖时间
            /// </summary>
            public long openTime { get; set; }

            /// <summary>
            /// 10秒取整之后的开奖时间
            /// </summary>
            public long OpenTime
            {
                get
                {
                    long time = this.openTime / 1000L;
                    time -= time % 10;
                    if (time % 40 != 0) return 0;
                    return time * 1000;
                }
            }

            /// <summary>
            /// 开奖期号
            /// </summary>
            public string Index => WebAgent.GetTimestamps(this.OpenTime).ToString("yyyyMMddHHmmss");
        }
    }
}
