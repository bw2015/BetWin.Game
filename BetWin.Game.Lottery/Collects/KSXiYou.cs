using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("快手-西游夺宝")]
    internal class KSXiYou : CollectProviderBase
    {
        public string gateway { get; set; } = "https://treasure.jwetech.com/main/game/info";

        public string api { get; set; } = "p_1685524313";

        public int user_id { get; set; } = 114990;

        public string sessid { get; set; } = "edc97db0df63ff970e946d7ad3f6131c";

        public string version { get; set; } = "1.0.6";

        public string security_key { get; set; } = "64770f5f34511";

        public KSXiYou(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        /// <summary>
        /// 使用一个静态变量存储要请求的game_id
        /// </summary>
        private static string? game_id = null;

        public override IEnumerable<CollectData> Execute()
        {
            var data = new Dictionary<string, object>()
                {
                    {"box_type","10000" },
                    {"get_next_gameinfo","0" },
                    {"sessid",this.sessid },
                    {"user_id",this.user_id }
                };

            if (!string.IsNullOrEmpty(game_id))
            {
                //data.Add("game_id", game_id);
            }
            this.getSign(data);

            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Post(this.gateway, data.ToQueryString(), new Dictionary<string, string>()
                {
                    {"Content-Type","application/x-www-form-urlencoded" },
                    {"User-Agent","com_kwai_gif/11.11.10 (iPhone; iOS 17.1.2; Scale/3.00)" }
                });
                //Console.WriteLine(result.Content);
                if (!result) yield break;

                JObject info = JObject.Parse(result.Content);
                int iRet = info["iRet"]?.Value<int>() ?? -1;
                if (iRet != 1) yield break;

                JToken? levelData = info["data"]?["level_1"]?.Value<JToken>();
                if (levelData == null || levelData.Type == JTokenType.Array) yield break;

                JObject? detail = levelData["detail"]?.Value<JObject>();
                if (detail == null) yield break;

                string index = detail["game_id"]?.Value<string>() ?? string.Empty;
                long openTime = detail["game_end"]?.Value<long>() ?? 0L;
                string[] winner = (detail["winner"]?.Value<JArray>().Select(t => t?.Value<string>()?.Replace("role_", "") ?? string.Empty).ToArray() ?? Array.Empty<string>()).Where(t => !string.IsNullOrEmpty(t)).ToArray();

                if (!string.IsNullOrEmpty(index))
                {
                    game_id = index;
                }

                if (string.IsNullOrEmpty(index) || openTime == 0 || !winner.Any()) yield break;

                yield return new CollectData(index, string.Join(",", winner), openTime * 1000L);
            }
        }

        /// <summary>
        /// 创建签名
        /// </summary>
        /// <returns></returns>
        private void getSign(Dictionary<string, object> data)
        {
            long signTime = WebAgent.GetTimestamps() / 1000L;
            foreach (var item in new SortedDictionary<string, object>()
            {
                {"sign_time",signTime },
                {"api",this.api },
                {"version",this.version }
            })
            {
                data.Add(item.Key, item.Value);
            }

            data.Add("security_key", this.security_key);
            string signStr = string.Join("&", data.OrderBy(t => t.Key).Select(t => $"{t.Key}={t.Value}"));
            data.Add("sign", signStr.toMD5().ToLower());
            data.Remove("security_key");
        }
    }
}
