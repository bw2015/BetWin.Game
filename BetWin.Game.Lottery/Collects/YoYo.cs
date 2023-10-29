using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("Yo交友")]
    public class YoYo : CollectProviderBase
    {
        public override LotteryType Type => LotteryType.Smart;

        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "YY.NorhArk";

        public YoYo(string setting) : base(setting)
        {
        }

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

            JArray list = JArray.Parse(result);
            foreach (var item in list)
            {
                string? name = item?["name"]?.Value<string>();
                DateTime? openTime = item?["openTime"]?.Value<DateTime>();
                if (name == null || openTime == null) continue;

                string number = name switch
                {
                    "水怪" => "1",
                    "蝙蝠" => "2",
                    "巨龙" => "3",
                    "泰坦" => "4",
                    "剑虎" => "5",
                    "冰熊" => "6",
                    "恶狼" => "7",
                    "火犬" => "8",
                    _ => "0"
                };

                string index = openTime.Value.ToString("yyyyMMddHHmm");
                yield return new CollectData(index, number, WebAgent.GetTimestamps(openTime.Value.AddSeconds(45)));
            }

            // 45秒的时候开奖
            long start = WebAgent.GetTimestamps(DateTime.Now.AddSeconds(DateTime.Now.Second * -1)),
                end = WebAgent.GetTimestamps(DateTime.Now.AddSeconds(DateTime.Now.Second * -1).AddSeconds(45));
            string betIndex = DateTime.Now.ToString("yyyyMMddHHmm");

            this.handler?.SaveStepTime(this.lotteryCode, new StepTimeModel(start, end, end, betIndex));
        }

    }
}
