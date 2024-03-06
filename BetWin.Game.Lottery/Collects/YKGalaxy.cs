using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("映客-银河探险")]
    public class YKGalaxy : CollectProviderBase
    {
        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "YK.Galaxy";

        public YKGalaxy(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            string url = $"{this.gateway}?key={this.key}";
            List<CollectData> list = new List<CollectData>();
            using (HttpClient client = new HttpClient())
            {
                string content = client.Get(url, new Dictionary<string, string>());
                item[]? items = content.ToJson<item[]>();
                if (items == null) return list;

                foreach (item item in items)
                {
                    list.Add(new CollectData(item.index, this.getNumber(item.name), WebAgent.GetTimestamps(item.openTime)));
                }
            }

            // 得到当前时间的可投注期
            DateTime now = DateTime.Now.Date.AddMinutes((int)DateTime.Now.TimeOfDay.TotalMinutes);
            if(now.Second > 50)
            {
                now = now.AddMinutes(1);
            }

            this.handler?.SaveIndexTime(this.lotteryCode,
                new StepTimeModel(new item()
                {
                    time = now.ToString("MM月dd日 HH:mm")
                }.index, WebAgent.GetTimestamps(now.AddSeconds(50)), WebAgent.GetTimestamps(now)));

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
                "土星" => "3",
                "天王星" => "4",
                "海王星" => "5",
                "开普勒星" => "6", 
                _ => "0"
            };
        }


        class item
        {
            public string name { get; set; }

            public string time { get; set; }

            public DateTime openTime
            {
                get
                {
                    return DateTime.Parse(string.Concat(DateTime.Now.Year, "年", this.time)).AddSeconds(50);
                }
            }

            public string index
            {
                get
                {
                    string date = this.openTime.ToString("yyyyMMdd");
                    int minute = (int)this.openTime.TimeOfDay.TotalMinutes;
                    return string.Concat(date, minute.ToString().PadLeft(4, '0'));
                }
            }

        }
    }
}
