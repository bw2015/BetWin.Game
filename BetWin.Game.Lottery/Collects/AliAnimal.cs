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
    [Description("动物运动会")]
    internal class AliAnimal : CollectProviderBase
    {
        public string url { get; set; } = "http://api.a8.to/Common/API_GetData?key=Ali.Animal";



        public AliAnimal(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        private static HttpClient client = new HttpClient();

        public override IEnumerable<CollectData> Execute()
        {
            List<CollectData> list = new List<CollectData>();

            token? token = this.getToken();
            if (token == null) return list;


            HttpClientResponse result = client.Get(token.gateway, new Dictionary<string, string>()
                {
                    {"Cookie",token.cookie },
                    {"User-Agent","Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.6 Mobile/15E148 Safari/604.1" }
                });
            //Console.WriteLine(result.Content);
            if (!result) return list;

            string? content = this.getContent(result);

            if (content == null) return list;

            response? res = content.ToJson<response>();
            if (res == null) return list;

            foreach (record record in res?.data?.record ?? Array.Empty<record>())
            {
                if (record.issue == null) continue;
                if (record.result == null) continue;

                list.Add(new CollectData(record.issue,
                    string.Join(",", record.result.Select(t => this.getNumber(t))),
                    this.getOpenTime(record.issue)));
            }

            if (list.Any())
            {
                CollectData data = list.FirstOrDefault();

                DateTime nextTime = WebAgent.GetTimestamps(data.OpenTime + 180 * 1000);
                int seconds = (int)nextTime.TimeOfDay.TotalSeconds + 20;
                int index = seconds / 180;

                this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel($"{nextTime:yyyyMMdd}{index.ToString().PadLeft(4, '0')}",
                    data.OpenTime + 180 * 1000,
                    data.OpenTime,
                    data.OpenTime + 170 * 1000));
            }

            return list;
        }

        /// <summary>
        /// 从远程接口读取最新的token内容
        /// </summary>
        /// <returns></returns>
        token? getToken()
        {
            var result = client.Get(this.url, new Dictionary<string, string>());
            return result.Content.ToJson<token>();
        }

        string? getContent(string result)
        {
            Regex regex = new Regex(@"mtopjsonp\d+\((?<Content>.+)\)");
            if (!regex.IsMatch(result)) return null;

            return regex.Match(result).Groups["Content"].Value;
        }

        /// <summary>
        /// 根据期号得到开奖时间
        /// </summary>
        long getOpenTime(string issue)
        {
            Regex regex = new Regex(@"^(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})(?<Index>\d{4})$");
            if (!regex.IsMatch(issue)) return 0;

            Match match = regex.Match(issue);
            DateTime date = new DateTime(int.Parse(match.Groups["Year"].Value), int.Parse(match.Groups["Month"].Value), int.Parse(match.Groups["Day"].Value));
            int index = int.Parse(match.Groups["Index"].Value);

            return WebAgent.GetTimestamps(date) + index * 1000 * 180 - 20 * 1000;

        }

        string getNumber(string number)
        {
            return number switch
            {
                // 饿小宝
                "A" => "1",
                // 盒马
                "B" => "2",
                // 票票
                "C" => "3",
                // 虾仔
                "D" => "4",
                // 支小宝
                "E" => "5",
                // 欢猩
                "F" => "6",
                _ => "0"
            };
        }

        class token
        {
            public string gateway { get; set; }

            public string cookie { get; set; }
        }

        class response
        {
            public string? api { get; set; }

            public responstData? data { get; set; }
        }

        class responstData
        {
            public record[]? record { get; set; }
        }

        class record
        {
            /// <summary>
            /// 开奖号码
            /// </summary>
            public string[]? result { get; set; }

            /// <summary>
            /// 期号
            /// </summary>
            public string? issue { get; set; }

        }
    }
}
