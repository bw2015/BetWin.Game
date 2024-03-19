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
using System.Text.RegularExpressions;

namespace BetWin.Game.Lottery.Collects
{
    [Description("映客-银河探险")]
    public class YKGalaxy : CollectProviderBase
    {
        public string url { get; set; } = "http://api.a8.to/Common/API_GetData?key=YK.Galaxy";

        public string record { get; set; } = "http://api.a8.to/Common/API_GetData?key=YK.Galaxy.Records";

        public YKGalaxy(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            List<CollectData> list = new List<CollectData>();
            using (HttpClient client = new HttpClient())
            {
                string content = client.Get(url, new Dictionary<string, string>());
                response? res = content.ToJson<response>();
                if (res != null)
                {
                    string number = this.getNumber(res.name);
                    if (number != "0")
                    {
                        list.Add(new CollectData(res.index, number, WebAgent.GetTimestamps(res.openTime)));
                    }
                }

                content = client.Get(record, new Dictionary<string, string>());
                item[]? items = content.ToJson<item[]>();
                foreach (item item in items ?? Array.Empty<item>())
                {
                    string number = this.getNumber(item.name);
                    if (number == "0") continue;
                    list.Add(new CollectData(item.index, number, WebAgent.GetTimestamps(item.openTime)));
                }
            }

            // 根据当前期计算出可投注的数据
            CollectData? data = list.FirstOrDefault();
            if (data != null)
            {
                DateTime nextOpenTime = WebAgent.GetTimestamps(data.Value.OpenTime).AddMinutes(1);

                this.handler?.SaveIndexTime(this.lotteryCode,
                new StepTimeModel(nextOpenTime.ToString("yyyyMMddHHmm"), WebAgent.GetTimestamps(nextOpenTime), WebAgent.GetTimestamps(nextOpenTime.AddSeconds(-50)))
                );

            }
            return list;
        }

        private string getNumber(string name)
        {
            //{"id":1,"name":"金星","weight":0,"multiple":5},
            //{"id":2,"name":"木星","weight":0,"multiple":5},
            //{"id":3,"name":"水星","weight":0,"multiple":5},
            //{"id":4,"name":"火星","weight":0,"multiple":5},
            //{"id":5,"name":"土星","weight":0,"multiple":10},
            //{"id":6,"name":"天王星","weight":0,"multiple":15},
            //{"id":7,"name":"海王星","weight":0,"multiple":25},
            //{"id":8,"name":"开普勒星","weight":0,"multiple":45}
            //小倍数12-78。大倍3456
            return name switch
            {
                "水星" => "1",
                "金星" => "2",
                "火星" => "7",
                "木星" => "8",
                "土星" => "6",
                "天王星" => "5",
                "海王星" => "4",
                "开普勒星" => "3",
                _ => "0"
            };
        }

        class response
        {
            public long time { get; set; }

            public string? content { get; set; }

            public string name
            {
                get
                {
                    if (string.IsNullOrEmpty(this.content)) return string.Empty;
                    Regex regex = new Regex(@"(?<Name>.+)发现大量宝藏");
                    if (!regex.IsMatch(this.content)) return string.Empty;
                    return regex.Match(this.content).Groups["Name"].Value;
                }
            }

            /// <summary>
            /// 推算出的开奖时间
            /// </summary>
            public DateTime openTime
            {
                get
                {
                    DateTime now = WebAgent.GetTimestamps(this.time);
                    int times = ((int)now.TimeOfDay.TotalSeconds - 50) / 60;

                    return now.Date.AddSeconds(times * 60 + 50);
                }
            }

            /// <summary>
            /// 计算出的奖期
            /// </summary>
            public string index
            {
                get
                {
                    return this.openTime.ToString("yyyyMMddHHmm");
                }
            }
        }

        class item
        {
            public string star { get; set; }

            public string time { get; set; }

            public DateTime openTime
            {
                get
                {
                    return DateTime.Parse(string.Concat(DateTime.Now.Year, "年", this.time)).AddSeconds(50);
                }
            }

            /// <summary>
            /// 星球名称
            /// </summary>
            public string name
            {
                get
                {
                    Regex regex = new Regex(@"宝藏星球：(?<Name>.+)（");
                    if (!regex.IsMatch(this.star)) return string.Empty;
                    return regex.Match(this.star).Groups["Name"].Value;
                }
            }
            public string index
            {
                get
                {
                    return this.openTime.ToString("yyyyMMddHHmm");
                }
            }
        }
    }
}
