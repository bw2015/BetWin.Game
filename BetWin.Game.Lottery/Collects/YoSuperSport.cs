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

namespace BetWin.Game.Lottery.Collects
{
    [Description("超能运动会")]
    internal class YoSuperSport : CollectProviderBase
    {
        public string url { get; set; } = "http://api.a8.to/Common/API_GetData?key=Yo.Super";

        public YoSuperSport(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

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
            string result = Client.Get(this.url, new Dictionary<string, string>());
            if (string.IsNullOrEmpty(result)) yield break;

            response[]? list = result.ToJson<response[]>();
            if (list == null || list.Length == 0) yield break;
            foreach (response res in list)
            {
                if (res.OpenTime == 0) yield break;
                yield return new CollectData(res.Index, this.getNumber(res.animal_id), res.OpenTime);
            }

            response newResult = list.FirstOrDefault();
            this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(
                 WebAgent.GetTimestamps(newResult.OpenTime + 40 * 1000).ToString("yyyyMMddHHmmss"),
                 newResult.OpenTime + 40 * 1000,
                 newResult.OpenTime,
                 newResult.OpenTime + 35 * 1000
                ));
        }

        private string getNumber(int animal_id)
        {

            //{id:1,name:"迅影猴",
            //id:5,name:"闪电鼠",
            //id:2,name:"滚石猫"
            //id:4,name:"奔雷虎"
            //id:6,name:"火箭龟"
            //id:3,name:"流星兔"
            return animal_id switch
            {
                // 迅影猴
                1 => "1",
                // 滚石猫
                2 => "2",
                // 流星兔
                3 => "3",
                // 奔雷虎
                4 => "4",
                // 闪电鼠
                5 => "5",
                // 火箭龟
                6 => "6",

                _ => "0"
            };
        }


        class response
        {
            /// <summary>
            /// 开奖号码
            /// </summary>
            public int animal_id { get; set; }

            /// <summary>
            /// 开奖时间（秒）
            /// </summary>
            public long timestamp { get; set; }

            /// <summary>
            /// 10秒取整之后的开奖时间
            /// </summary>
            public long OpenTime => this.timestamp * 1000L + 40 * 1000L;

            /// <summary>
            /// 开奖期号
            /// </summary>
            public string Index => WebAgent.GetTimestamps(this.OpenTime).ToString("yyyyMMddHHmmss");
        }
    }
}
