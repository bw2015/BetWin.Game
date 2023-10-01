using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("永利数据")]
    public class YongLi : CollectProviderBase
    {
        public string gateway { get; set; } = "http://yongli.haojiajiabaihuo.xyz/qishu/open_log";

        /// <summary>
        /// 游戏类型 5:诺亚方舟
        /// </summary>
        public int gameType { get; set; } = 5;

        public string token { get; set; } = "964c1ed504bd7d2c0533fbc93e464428";

        public YongLi(string setting) : base(setting)
        {
        }

        private HttpClient client = new HttpClient();

        public override IEnumerable<CollectData> Execute()
        {
            var result = client.Post(this.gateway, JsonConvert.SerializeObject(new
            {
                page = 0,
                game_type = this.gameType
            }), new Dictionary<string, string>()
            {
                {"user-login-token", token},
                {"Content-Type","application/json" },
                {"Accept-Encoding","gzip" },
                {"User-Agent","Mozilla/5.0 (Linux; Android 12; PGJM10 Build/V417IR; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/91.0.4472.114 Mobile Safari/537.36 uni-app Html5Plus/1.0" }
            });

            if (!result) yield break;

            JObject info = JObject.Parse(result);
            JArray? list = info?["data"]?.Value<JArray>();
            if (list == null) yield break;

            foreach (var item in list)
            {
                long? openTime = item["open_time"]?.Value<long>();
                string? number = item["number"]?.Value<string>();
                if (openTime == null || number == null) continue;
                yield return new CollectData(openTime.ToString(), number, openTime.Value * 1000L);
            }

            DateTime start = DateTime.Now.AddMinutes(DateTime.Now.Second * -1),
                end = start.AddSeconds(59);
            string index = (end.GetTimestamps() / 1000L).ToString();
            this.handler?.SaveStepTime(this.lotteryCode ?? string.Empty, 
                new StepTimeModel(
                    WebAgent.GetTimestamps(start),
                    WebAgent.GetTimestamps(end),
                    index
                    ));
        }
    }
}
