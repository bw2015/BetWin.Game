﻿using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using SP.StudioCore.API.Baidu;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    /// <summary>
    /// 脚本中转采集
    /// </summary>
    [Description("抓鸭子")]
    public class SKRDuck : CollectProviderBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string gateway { get; set; } = "http://api.a8.to/Common/API_GetData";

        public string key { get; set; } = "SKR.duck";

        public string app_key { get; set; } = "ajWI3G06OsttwChmBwhQ6Rob";

        public string app_secret { get; set; } = "wiQ1HOqOxA3nfbpdv6GgQuFXTATIX2x2";

        public SKRDuck(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        private static long lastTime = 0;

        public override IEnumerable<CollectData> Execute()
        {
            string url = $"{this.gateway}?key={this.key}";
            List<CollectData> list = new List<CollectData>();
            long now = WebAgent.GetTimestamps() / 1000L;
            if (now - lastTime < 30) return list;

            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(url, new Dictionary<string, string>()
                {
                    {"accept","application/json, text/plain, */*" },
                    {"sec-fetch-site","same-origin" },
                    {"inframe-client-id","39E53030" },
                    {"accept-encoding","gzip, deflate, br" },
                    {"sec-fetch-mode","cors" },
                    {"user-agent","Mozilla/5.0 (iPhone; CPU iPhone OS 17_1_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 pid/39E53030-2721-4381-B06B-2F420EBED052 NetType/WiFi Pixel/1242 appVersion/5008003 DeviceName/iPhone XS Max CN" },
                    {"sec-fetch-dest","empty" }
                });

                if (!result) return list;
                string content = result.Content;
                if (content.StartsWith("["))
                {
                    content = string.Concat("{\"hasMore\":true,\"offset\":0,\"items\":", content, "}");
                }

                response? response = content.ToJson<response>();

                if (response?.items == null) return list;

                foreach (var item in response.items)
                {
                    string duckName = this.getNumber(item.duckName);
                    if (duckName == "0") continue;
                    list.Add(new CollectData(item.gameID.ToString(), this.getNumber(item.duckName), item.createdTime * 1000L));
                }

                lastTime = response.items.Max(t => t.createdTime);
                int gameId = response.items.Max(t => t.gameID);

                this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel((gameId + 1).ToString(), (lastTime + 30) * 1000L, lastTime * 1000L));

                return list;
            }
        }

        private string getNumber(string? name)
        {
            name = this.getDuckName(name);
            return name switch
            {
                "划水鸭" => "1",   // 1.6%
                "拴Q鸭" => "2",   // 3%
                "厉害鸭" => "3",   // 7%
                "小龙鸭" => "4",   // 8%
                "心动鸭" => "5",   // 21%
                "唱歌鸭" => "6",   // 17.5% 
                "干饭鸭" => "7",   // 10.6%
                "吃瓜鸭" => "8",   // 31.3%
                _ => "0"
            };
        }

        static Dictionary<string, string> cache = new Dictionary<string, string>();

        /// <summary>
        /// 使用OCR进行识别
        /// </summary>
        private string? getDuckName(string? file)
        {
            if (file == null || !file.EndsWith(".png")) return file;
            file = $"https://res-static.inframe.mobi/ui/{file}";
            if (cache.ContainsKey(file)) return cache[file];

            IEnumerable<string> words = new BaiduClient(this.app_key, this.app_secret).getContent(file);
            string content = string.Join(string.Empty, words);
            if (words.Any() || !string.IsNullOrEmpty(content))
            {
                cache.Add(file, content);
            }
            Console.WriteLine($"{file},{content}");
            return content;
        }


        class response
        {
            public bool hasMore { get; set; }

            public int offset { get; set; }

            public responseItem[]? items { get; set; }
        }

        class responseItem
        {
            public int gameID { get; set; }

            /// <summary>
            /// 开奖时间（秒）
            /// </summary>
            public long createdTime { get; set; }

            public string? duckUrl { get; set; }

            /// <summary>
            /// 鸭子名字
            /// </summary>
            public string? duckName { get; set; }

            /// <summary>
            /// 开奖号码
            /// </summary>
            public int position { get; set; }
        }
    }
}
