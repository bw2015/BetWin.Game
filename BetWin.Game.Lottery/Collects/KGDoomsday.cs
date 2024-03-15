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
using System.Text.RegularExpressions;

namespace BetWin.Game.Lottery.Collects
{
    /// <summary>
    /// 酷狗唱唱 - 末日保卫战
    /// </summary>
    [Description("酷狗-末日保卫战")]
    public class KGDoomsday : CollectProviderBase
    {
        /// <summary>
        /// 实时开奖的API地址
        /// </summary>
        public string Url { get; set; } = "http://api.a8.to/Common/API_GetData?key=KG.Doomsday";

        /// <summary>
        /// 开奖记录接口地址
        /// </summary>
        public string logUrl { get; set; } = "https://acsing.service.kugou.com/sing7/singlematch/json/v3/tower_defence/monster_history?playerId=1065149824&appid=2858&srcappid=3148&clientver=48800&token=h5022FAA27445EB1F5B570151CCFB299D64DC42F9ABC28AB3AC5342738EFAFA312062F94EA733A002D697E10781C6FD09513F81DA2C31BB0B61968A0761C73DDF89B6527E03135369FFA845A6317EBF363982C547944C63818B08C84FE1788472358187C0F79E86CED741F695B99DD4A3B6E2A477030F243F72975D4952AFC8FA3&signature=fec65e59ba819a82ae56dffbc1ffbad5";


        public string uuid { get; set; } = "9b1274c1c0e67466dd0275b024ab44a1c99412bb";

        public string dfid { get; set; } = "2B5CdN2vJwnv0DxEsw4DTyp3";

        public string pid { get; set; } = "1065149824";

        public KGDoomsday(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        /// <summary>
        /// a.a.AES.decrypt(e,a.a.enc.Utf8.parse(u),{iv:a.a.enc.Utf8.parse(g),mode:a.a.mode.CBC,padding:a.a.pad.Pkcs7}).toString(a.a.enc.Utf8))
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<CollectData> Execute()
        {
            var list = new List<CollectData>();

            CollectData? current = this.getCurrent();
            if (current != null)
            {
                list.Add(current.Value);
            }

            var logs = this.getLogs().Where(t => t.Index != current?.Index);
            list.AddRange(logs);

            // 根据当前期得到下一期的数据
            CollectData? bet = list.FirstOrDefault();
            if (bet != null)
            {
                string? nextIndex = this.getNextIndex(bet.Value.Index);
                if (nextIndex != null)
                {

                    this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(nextIndex, bet.Value.OpenTime + 50 * 1000, bet.Value.OpenTime + 15 * 1000));

                }
            }

            return list.Where(t => t.Number != "0");
        }

        private string? getNextIndex(string roundId)
        {
            Regex regex = new Regex(@"^(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})(?<Hour>\d{2})(?<Minute>\d{2})(?<Second>\d{2})$");
            if (!regex.IsMatch(roundId)) return null;

            var group = regex.Match(roundId).Groups;
            DateTime openTime = DateTime.Parse($"{group["Year"].Value}-{group["Month"].Value}-{group["Day"].Value} {group["Hour"].Value}:{group["Minute"].Value}:{group["Second"].Value}");

            DateTime nextTime = openTime.AddSeconds(50);
            return nextTime.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 获取当前期的开奖号码
        /// </summary>
        /// <returns></returns>
        private CollectData? getCurrent()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(this.Url, new Dictionary<string, string>());
                if (string.IsNullOrEmpty(result.Content)) return null;
                JObject info = JObject.Parse(result);

                string roundId = info["data"]?["roundId"]?.Value<string>() ?? string.Empty;
                if (string.IsNullOrEmpty(roundId)) return null;

                string content = info["result"]?.Value<string>() ?? string.Empty;
                responseResult? res = content.ToJson<responseResult>();
                if (res == null) return null;

                return new CollectData(roundId, this.getNumber(res.monsterId), this.getOpenTime(roundId));
            }
        }

        /// <summary>
        /// 根据期号获取开奖时间
        /// </summary>
        private long getOpenTime(string roundId)
        {
            Regex regex = new Regex(@"^(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})(?<Hour>\d{2})(?<Minute>\d{2})(?<Second>\d{2})$");
            if (!regex.IsMatch(roundId)) return 0;

            var group = regex.Match(roundId).Groups;
            DateTime openTime = DateTime.Parse($"{group["Year"].Value}-{group["Month"].Value}-{group["Day"].Value} {group["Hour"].Value}:{group["Minute"].Value}:{group["Second"].Value}");

            return WebAgent.GetTimestamps(openTime) + 35 * 1000;
        }

        /// <summary>
        /// 获取历史开奖记录
        /// </summary>
        private IEnumerable<CollectData> getLogs()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(this.logUrl, this.headers());

                response? res = result.Content.ToJson<response>();
                if (res == null) yield break;

                foreach (var monster in res?.data?.monsterInfos ?? Array.Empty<monsterInfo>())
                {
                    yield return new CollectData(monster.roundId, this.getNumber(monster.monsterId), this.getOpenTime(monster.roundId));
                }
            }
        }

        /// <summary>
        /// 开奖号码转换
        /// </summary>
        private string getNumber(int monsterId)
        {
            return monsterId switch
            {
                //原始人 毒蘑菇 @ 喷火枪
                1 => "1",
                //蝙蝠 投石车 @ 投石车
                2 => "2",
                //钻石狂牛 熔岩 @ 弩机
                3 => "7",
                //火山咔咔 地狱魔瞳 @ 圣光之心
                4 => "8",
                //骇鸟 铁象牙 @ 铁蒺藜
                5 => "5",
                //长毛豚 食豚花 @ 剧毒屏障
                6 => "6",
                //赤毒 仙人球 @ 太阳权杖
                7 => "4",
                //乙龙 灭龙果 @ 猎龙驽
                8 => "3",
                _ => "0"
            };
        }

        private Dictionary<string, string> headers()
        {
            return new Dictionary<string, string>()
            {
                  {"uuid",uuid },
                    {"mid",uuid },
                    {"clienttime",(WebAgent.GetTimestamps()/1000L).ToString() + "000" },
                    {"dfid",this.dfid },
                    {"Origin","https://activity.kugou.com" },
                    {"User-Agent","Mozilla/5.0 (iPhone; CPU iPhone OS 17_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 - 48800 - KGBrowser KGWebKit/1.0 KTVBrowser" },
                    {"Referer","https://activity.kugou.com/" },
                    {"pid",this.pid }
            };
        }

        class response
        {
            public int code { get; set; }

            public string msg { get; set; }

            public responseData data { get; set; }
        }

        class responseData
        {
            public int limit { get; set; }

            public monsterInfo[] monsterInfos { get; set; }
        }

        class monsterInfo
        {
            public int monsterId { get; set; }

            public string monsterName { get; set; }

            public int fearWeaponId { get; set; }

            public string fearWeaponName { get; }

            public string roundId { get; set; }

            public long startTime { get; set; }

            public long endTime { get; set; }
        }

        class responseResult
        {
            public int monsterId { get; set; }
        }
    }
}
