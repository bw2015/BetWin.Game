using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Models;
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
    [Description("宠物马拉松")]
    public class HuYaPet : CollectProviderBase
    {
        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "HY.pet";

        public HuYaPet(string setting) : base(setting)
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

            pet[]? pets = JsonConvert.DeserializeObject<pet[]>(result);
            if (pets == null) yield break;

            this.handler?.SaveStepTime(this.lotteryCode, new StepTimeModel());

            foreach (var pet in pets)
            {
                if (pet.date == null || pet.date.Length != 2) continue;
                DateTime openTime = DateTime.Parse($"{DateTime.Now.Year}-{pet.date[0]} {pet.date[1]}");
                string index = openTime.ToString("yyyyMMddHHmm");

                if (pet.date?.Length == 2 && pet.locList?.Length == 0 && pet.odds?.Length == 7)
                {
                    if (pet.Odds != null)
                    {
                        this.handler?.SaveIndexData(this.lotteryCode, index, pet.Odds.ToDictionary(t => this.getNumber(t.Key), t => t.Value));
                    }
                }
                if (pet.locList == null || pet.locList.Length != 1) continue;

                if (openTime > DateTime.Now) openTime = openTime.AddYears(-1);
                string number = this.getNumber(pet.locList[0]);

                yield return new CollectData(index, number, WebAgent.GetTimestamps(openTime.AddMinutes(2)));
            }
        }

        string getNumber(string petName)
        {
            return petName switch
            {
                "飞机兔" => "1",
                "顽皮狗" => "2",
                "神秘龟" => "3",
                "科达鸭" => "4",
                "闪电猫" => "5",
                "百灵鸟" => "6",
                "大仓鼠" => "7",
                _ => "0"
            };
        }

        class pet
        {
            /// <summary>
            /// 开奖时间（到分钟）
            /// </summary>
            public string[]? date { get; set; }

            /// <summary>
            /// 开奖动物
            /// </summary>
            public string[]? locList { get; set; }

            /// <summary>
            /// 正在开奖期的赔率
            /// </summary>
            public string[]? odds { get; set; }

            [JsonIgnore]
            public Dictionary<string, int>? Odds
            {
                get
                {
                    string[] pets = new[]
                    {
                        "飞机兔","百灵鸟","大仓鼠","神秘龟","顽皮狗","科达鸭","闪电猫"
                    };
                    if (this.odds?.Length != pets.Length) return null;

                    Dictionary<string, int> data = new Dictionary<string, int>();
                    for (int index = 0; index < pets.Length; index++)
                    {
                        data.Add(pets[index], int.Parse(this.odds[index].Replace("x", "")));
                    }
                    return data;
                }
            }
        }
    }
}
