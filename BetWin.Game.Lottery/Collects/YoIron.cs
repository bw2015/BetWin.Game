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
    [Description("YO-钢铁统帅")]
    public class YoIron : CollectProviderBase
    {
        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "YY.Iron";

        public YoIron(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            List<CollectData> list = new List<CollectData>();
            string url = $"{this.gateway}?key={this.key}";
            using (HttpClient client = new HttpClient())
            {
                string content = client.Get(url, new Dictionary<string, string>());

                item[]? items = content.ToJson<item[]>();
                if (items == null) return list;

                foreach (item item in items)
                {
                    list.Add(new CollectData(item.index, item.number.ToString(), WebAgent.GetTimestamps(item.openTime.AddMinutes(3))));
                }
            }



            DateTime now = DateTime.Now;
            int minute = (int)now.TimeOfDay.TotalMinutes;
            minute -= (minute % 3);
            DateTime betTime = now.Date.AddMinutes(minute);

            item betItem = new item()
            {
                openTime = betTime
            };

            this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(betItem.index,
                WebAgent.GetTimestamps(betItem.openTime.AddMinutes(3)), WebAgent.GetTimestamps(betTime)));

            return list;
        }

        class item
        {
            public int number { get; set; }

            public DateTime openTime { get; set; }

            public string index
            {
                get
                {
                    string date = this.openTime.ToString("yyyyMMdd");
                    int minute = (int)this.openTime.TimeOfDay.TotalMinutes;

                    return string.Concat(date, ((minute / 3) + 1).ToString().PadLeft(3, '0'));
                }
            }
        }
    }
}
