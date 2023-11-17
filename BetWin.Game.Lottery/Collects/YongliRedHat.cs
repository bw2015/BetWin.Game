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
    [Description("小红帽(永利)")]
    internal class YongliRedHat : CollectProviderBase
    {
        public string gateway { get; set; } = "http://8.217.169.58/qishu/open_log";

        public string token { get; set; } = "3ebc8fec1651731538889cf8ad15c9ff";

        public string content { get; set; } = "{\"page\":0,\"game_type\":14}";

        public YongliRedHat(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.Post(this.gateway, this.content, new Dictionary<string, string>()
                {
                    {"Content-Type","application/json" },
                    {"user-login-token",this.token }
                });

                if (!response) yield break;

                // 只取最新的一期开奖
                JObject json = JObject.Parse(response);
                JToken? info = json?["data"]?.Value<JArray>().OrderByDescending(t => t["open_time"]?.Value<long>() ?? 0L).FirstOrDefault();
                if (info == null) yield break;

                string? number = info["number"]?.Value<string>();
                long? time = info["open_time"]?.Value<long>();
                if (number == null || time == null) yield break;

                long openTime = time.Value * 1000L;

                // 得到上一期的开奖时间，如果差距小于50秒，则退出
                string? index = this.handler?.GetBetIndex(this.lotteryCode, openTime, 55);
                if (index == null) yield break;

                CollectData data = new CollectData()
                {
                    Number = number,
                    OpenTime = openTime,
                    Index = index
                };

                // 根据最新一期的开奖时间计算出当前期的开奖时间
                string nextIndex = (int.Parse(index) + 1).ToString();
                long nextOpenTime = openTime + 55 * 1000L;

                this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(nextIndex, nextOpenTime, openTime));

                yield return data;
            }
        }


    }
}
