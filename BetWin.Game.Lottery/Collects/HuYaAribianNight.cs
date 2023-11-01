using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Utils;
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
    [Description("虎牙 一千零一夜")]
    internal class HuYaAribianNight : CollectProviderBase
    {
        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "HY.AribianNight";

        public HuYaAribianNight(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        private static HttpClient? _client;

        static HttpClient httpClient
        {
            get
            {
                return _client ??= new HttpClient();
            }
        }

        public override IEnumerable<CollectData> Execute()
        {
            string url = $"{this.gateway}?key={this.key}";
            string result = httpClient.Get(url, new Dictionary<string, string>());
            if (string.IsNullOrEmpty(result)) yield break;

            aribianNight[]? list = JsonConvert.DeserializeObject<aribianNight[]>(result);
            if (list == null || !list.Any()) yield break;

            foreach (aribianNight item in list)
            {
                if (item.date == null || item.date.Length != 2) continue;
                DateTime openTime = DateTime.Parse($"{DateTime.Now.Year}-{item.date[0]} {item.date[1]}");
                string index = openTime.ToString("yyyyMMddHHmm");
                if (item.locList == null || !item.locList.Any()) continue;

                if (openTime > DateTime.Now) openTime = openTime.AddYears(-1);
                string number = this.getNumber(item.locList);

                yield return new CollectData(index, number, WebAgent.GetTimestamps(this.getOpenTime(openTime)));
            }
        }

        /// <summary>
        /// 根据当前时间生成开奖时间
        /// </summary>
        /// <param name="openTime"></param>
        /// <returns></returns>
        private DateTime getOpenTime(DateTime openTime)
        {
            DateTime now = DateTime.Now;
            openTime = openTime.AddMinutes(3);
            if (now - openTime < TimeSpan.FromMinutes(1)) return openTime.AddSeconds(now.Second);
            if (now - openTime < TimeSpan.FromMinutes(1.5)) return now.AddSeconds(-3);
            return openTime;
        }

        string getNumber(string[] locList)
        {
            return string.Join(",", locList.Select(t => this.getNumber(t)));
        }

        string getNumber(string itemName)
        {
            return itemName switch
            {
                "水瓶" => "1",
                "双鱼" => "2",
                "白羊" => "3",
                "金牛" => "4",
                "双子" => "5",
                "巨蟹" => "6",
                "狮子" => "7",
                "处女" => "8",
                "天秤" => "9",
                "天蝎" => "10",
                "射手" => "11",
                "摩羯" => "12",
                _ => "0"
            };
        }

        class aribianNight
        {
            /// <summary>
            /// 开奖时间（到分钟）
            /// </summary>
            public string[]? date { get; set; }

            /// <summary>
            /// 开奖动物
            /// </summary>
            public string[]? locList { get; set; }
        }
    }
}
