using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    /// <summary>
    /// 虎牙宠物马拉松 2分钟一期
    /// </summary>
    [Description("宠物马拉松")]
    public class HuYaPet : CollectProviderBase
    {
        public override LotteryType Type => LotteryType.Smart;

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

            Console.WriteLine(result);

            pet[]? pets = JsonConvert.DeserializeObject<pet[]>(result);
            if (pets == null || !pets.Any()) yield break;


            foreach (pet pet in pets)
            {
                if (pet.date == null || pet.date.Length != 2) continue;

                DateTime? openTime = pet.getOpenTime();
                string? index = pet.getIndex();

                if (openTime == null || index == null) continue;

                // 如果是赔率推送
                if (pet.date?.Length == 2 && pet.locList?.Length == 0 && pet.odds?.Length == 7 && pet.openTime != null)
                {
                    if (pet.Odds != null)
                    {
                        // 保存自定义的赔率内容
                        this.handler?.SaveIndexData(this.lotteryCode, index, pet.Odds.ToDictionary(t => this.getNumber(t.Key), t => this.getOdds(t.Value)));

                        // 如果是开奖期
                        this.handler?.SaveIndexTime(this.lotteryCode,
                            new StepTimeModel(index,
                            pet.openTime.Value,
                            pet.openTime.Value - 120 * 1000));

                        // 保存开奖时间到本地缓存
                        saveOpenTime(index, pet.openTime.Value);
                    }
                }

                // 如果开奖动物没有数据
                if (pet.locList == null) continue;

                string? openNumber = pet.getNumber();

                if (string.IsNullOrEmpty(openNumber)) continue;
                if (openTime > DateTime.Now) openTime = openTime.Value.AddYears(-1);
                string number = this.getNumber(openNumber);

                yield return new CollectData(index, number, getOpenTime(index, openTime.Value));
            }
        }

        /// <summary>
        /// 赔率转换
        /// </summary>
        /// <param name="odds"></param>
        /// <returns></returns>
        private int getOdds(int odds)
        {
            return odds switch
            {
                //3 => 2,
                _ => odds
            };
        }

        #region ========  开奖时间静态缓存  ========


        private static ConcurrentDictionary<string, long> openTimeCache = new ConcurrentDictionary<string, long>();

        /// <summary>
        /// 写入开奖时间
        /// </summary>
        private static void saveOpenTime(string index, long openTime)
        {
            if (openTimeCache.ContainsKey(index)) return;

            long now = WebAgent.GetTimestamps();

            // 超过30分钟的删除
            foreach (string timeIndex in openTimeCache.Keys.ToArray())
            {
                long time = openTimeCache[timeIndex];
                if (now - time > 30 * 60 * 1000)
                {
                    openTimeCache.TryRemove(timeIndex, out time);
                }
            }

            if (!openTimeCache.ContainsKey(index)) openTimeCache.TryAdd(index, openTime);
        }

        /// <summary>
        /// 从缓存中获取开奖时间
        /// </summary>
        private static long getOpenTime(string index, DateTime openTime)
        {
            if (openTimeCache.TryGetValue(index, out long time))
            {
                return time;
            }
            return WebAgent.GetTimestamps(openTime);
        }

        #endregion

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
            /// 开始时间
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

            /// <summary>
            /// 开奖时间
            /// </summary>
            public long? openTime { get; set; }

            /// <summary>
            /// 获取当前期
            /// </summary>
            /// <returns></returns>
            public string? getIndex()
            {
                DateTime? openTime = this.getOpenTime();
                if (openTime == null) return null;
                return openTime.Value.ToString("yyyyMMddHHmm");
            }

            public DateTime? getOpenTime()
            {
                if (this.date == null || this.date.Length != 2) return null;
                DateTime openTime = DateTime.Parse($"{DateTime.Now.Year}-{this.date[0]} {this.date[1]}");
                return openTime;
            }

            /// <summary>
            /// 获取开奖号码
            /// </summary>
            public string? getNumber()
            {
                if (this.locList == null || this.locList.Length != 1) return null;
                return this.locList[0];
            }

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
