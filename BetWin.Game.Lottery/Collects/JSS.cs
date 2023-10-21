using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("金闪闪")]
    public class JSS : CollectProviderBase
    {
        public override LotteryType Type => LotteryType.Smart;

        public string Gateway { get; set; } = "https://amc.kmactoer.cn/lottery/history";

        public string Token { get; set; } = "779f0023-068d-40e9-b27b-8899fe700367";

        [Description("彩种")]
        public string assort { get; set; } = "KSXHM:XHMHJ";

        public JSS(string setting) : base(setting)
        {
        }

        public override IEnumerable<CollectData> Execute()
        {
            HttpClient client = new HttpClient();

            string result = client.Request(new HttpClientRequest()
            {
                Url = this.Gateway,
                Method = HttpMethod.Post,
                Encoding = Encoding.UTF8,
                Headers = new Dictionary<string, string>()
                    {
                        { "user-agent","Dart/3.0 (dart:io)" },
                        {"token",this.Token},
                        {"content-type","application/json" }
                    },
                PostData = new requestData
                {
                    assort = assort
                }.ToString()
            });

            //{
            //    "code": 1,
            //    "summary": "ok",
            //    "data": "1692374528|1\u00261692374473|1\u00261692374418|1",
            //    "time": 1692374568
            //}
            string? data = JObject.Parse(result)?["data"]?.Value<string>();
            if (data == null) yield break;

            foreach (string item in data.Split('&'))
            {
                string[] items = item.Split('|');

                long openTime = long.Parse(items[0]) * 1000L;
                string number = items[1];

                DateTime dateTime = WebAgent.GetTimestamps(openTime);
                int minute = dateTime.Hour * 60 + dateTime.Minute + 1;

                yield return new CollectData()
                {
                    Index = string.Concat(dateTime.ToString("yyyyMMdd"), minute.ToString().PadLeft(4, '0')),
                    OpenTime = openTime,
                    Number = number
                };
            }
        }

        #region ========  内部实体类  ========

        class requestData
        {
            public string? assort { get; set; }

            public int type { get; set; } = 2;

            public string[] cols { get; set; } = new string[] { "start_time", "nums" };

            public string[] seps { get; set; } = new string[] { "|", "&" };

            public long issue_time { get; set; } = WebAgent.GetTimestamps() / 1000L;

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        class responseData
        {

        }

        #endregion
    }
}
