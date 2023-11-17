using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// 采集火箭发射
    /// </summary>
    [Description("全民K歌")]
    public class QMKG : CollectProviderBase
    {

        public override LotteryType Type => LotteryType.Smart;
        public string x_gopen_id { get; set; } = "OGr65LJSYMXg8PGV-j4pnZw9-oiA";

        public string x_gopen_session_key { get; set; } = "AQFuybaomg3hZNhBsM+EXQ";

        public string x_gopen_app_id { get; set; } = "20000051";

        /// <summary>
        /// 历史开奖记录的网关
        /// </summary>
        public string historyUrl { get; set; } = "https://apigame.kg.qq.com/kg.wh_rocket.Rocket/GetAllPlayUser";

        /// <summary>
        /// 获取开奖历史记录的配置内容   ts=xxx&sign=xxx
        /// </summary>
        public string historySetting { get; set; } = "{\"ts\":\"1692621130\",\"sign\":\"174224129269035cd81521613fe3fc0686fdee0dde46ea33b9749a354f695b90\"}";

        /// <summary>
        /// 开奖时间的网关
        /// </summary>
        public string playTimeUrl { get; set; } = "https://apigame.kg.qq.com/kg.wh_rocket.Rocket/GetPlayTime";

        /// <summary>
        /// 获取开奖时间的设置内容  ts=xxx&sign=xxx
        /// </summary>
        public string playTimeSetting { get; set; } = "{\"ts\":\"1692621141\",\"sign\":\"ac1c7850087f08c7abd0819dbdecb15bcf5d9f1f5c4251f2691c22051c652e25\"}";

        /// <summary>
        /// 获取开奖的接口
        /// </summary>
        public string getPlayUserResultUrl { get; set; } = "https://apigame.kg.qq.com/kg.wh_rocket.Rocket/GetPlayUserResult";

        /// <summary>
        /// 实时开奖接口的签名配置
        /// </summary>
        public string getPlayUserResultSetting { get; set; } = "{\"ts\":\"1692621141\",\"sign\":\"ac1c7850087f08c7abd0819dbdecb15bcf5d9f1f5c4251f2691c22051c652e25\"}";

        public QMKG(string setting) : base(setting)
        {
        }

        private HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(3)
        };

        public override IEnumerable<CollectData> Execute()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                {this.getHeaderKey(nameof(x_gopen_id)), this.x_gopen_id },
                {this.getHeaderKey(nameof(x_gopen_session_key)), this.x_gopen_session_key },
                {this.getHeaderKey(nameof(x_gopen_app_id)), this.x_gopen_app_id },
                {"Content-Type","application/json" },
                {"Accept","application/json" },
                {"Accept-Encoding","gzip, deflate, br" },
                {"x-requested-with","com.tencent.karaoke" }
            };

            var playTime = this.getPlayTime(client, headers);
            this.handler?.SaveIndexTime(this.lotteryCode, playTime);

            // 如果当前到了开奖时间则获取实时接口
            //Console.WriteLine($"{playTime.endTime} < {WebAgent.GetTimestamps()} => {playTime.endTime < WebAgent.GetTimestamps()}");
            if (playTime.endTime < WebAgent.GetTimestamps())
            {
                CollectData data = this.getPlayUserResult(client, playTime, headers);
                if (data)
                {
                    //Console.WriteLine($"采集到数据 => {JsonConvert.SerializeObject(data)}");
                    yield return data;
                }
            }
            else
            {
                foreach (var item in this.getHistory(client, headers).ToArray())
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 读取历史记录接口
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CollectData> getHistory(HttpClient client, Dictionary<string, string> headers)
        {
            headers = JsonConvert.DeserializeObject<signSetting>(this.historySetting)?.getHeaders(headers) ?? new Dictionary<string, string>();

            HttpClientResponse result = client.Request(new HttpClientRequest()
            {
                Url = this.historyUrl,
                Encoding = Encoding.UTF8,
                Headers = headers,
                Method = HttpMethod.Post,
                PostData = "{}"
            });

            //Console.WriteLine($"getHistory => {result.Content}");

            if (!result) yield break;

            JObject info = JObject.Parse(result);

            JArray? data = info?["data"]?.Value<JArray>();
            if (data == null) yield break;
            foreach (var item in data)
            {
                long? time = item["playTime"]?.Value<long>();
                string? number = item["roleFlag"]?.Value<string>();

                if (time == null || number == null) continue;

                yield return new CollectData()
                {
                    Index = WebAgent.GetTimestamps(time.Value).ToString("yyyyMMddHHmm"),
                    Number = number,
                    OpenTime = time.Value + 1000 * 60
                };
            }
        }

        /// <summary>
        /// 获取开奖时间的配置
        /// </summary>
        private StepTimeModel getPlayTime(HttpClient client, Dictionary<string, string> headers)
        {
            headers = JsonConvert.DeserializeObject<signSetting>(this.playTimeSetting)?.getHeaders(headers) ?? new Dictionary<string, string>();
            HttpClientResponse result = client.Request(new HttpClientRequest()
            {
                Url = this.playTimeUrl,
                Encoding = Encoding.UTF8,
                Headers = headers,
                Method = HttpMethod.Post,
                PostData = "{}"
            });

            //Console.WriteLine($"getPlayTime => {result.Content}");

            JObject info = JObject.Parse(result);

            string betIndex = WebAgent.GetTimestamps(info["gameStartTime"]?.Value<long>() ?? 0L).ToString("yyyyMMddHHmm");
            long startTime = info["gameStartTime"]?.Value<long>() ?? 0L,
                  endTime = info["gameEndTime"]?.Value<long>() ?? 0L,
                openTime = info["gameResultTime"]?.Value<long>() ?? 0L;
            return new StepTimeModel(betIndex, openTime, startTime, endTime);
        }

        private CollectData getPlayUserResult(HttpClient client, StepTimeModel time, Dictionary<string, string> headers)
        {
            headers = JsonConvert.DeserializeObject<signSetting>(this.getPlayUserResultSetting)?.getHeaders(headers) ?? new Dictionary<string, string>();
            HttpClientResponse result = client.Request(new HttpClientRequest()
            {
                Url = this.getPlayUserResultUrl,
                Encoding = Encoding.UTF8,
                Headers = headers,
                Method = HttpMethod.Post,
                PostData = "{}"
            });

            //Console.WriteLine($"getPlayUserResult => {result.Content}");

            if (!result) return default;


            JObject info = JObject.Parse(result);
            string? winRole = info["winRole"]?.Value<string>();
            if (winRole == null) return default;

            return new CollectData()
            {
                Index = WebAgent.GetTimestamps(time.startTime).ToString("yyyyMMddHHmm"),
                Number = winRole,
                OpenTime = time.startTime + 1000 * 60
            };
        }

        private string getHeaderKey(string key)
        {
            return key.Replace('_', '-');
        }

        /// <summary>
        /// 签名配置
        /// </summary>
        class signSetting
        {
            [JsonProperty("sign")]
            public string x_gopen_sign { get; set; } = "";

            [JsonProperty("ts")]
            public string x_gopen_ts { get; set; } = "";

            public Dictionary<string, string> getHeaders(Dictionary<string, string> headers)
            {
                var data = new Dictionary<string, string>()
                {
                    {"x-gopen-sign",this.x_gopen_sign },
                    {"x-gopen-ts", this.x_gopen_ts }
                };
                foreach (var item in data)
                {
                    if (headers.ContainsKey(item.Key))
                    {
                        headers[item.Key] = item.Value;
                    }
                    else
                    {
                        headers.Add(item.Key, item.Value);
                    }
                }

                return headers;
            }
        }

        /*
        {
            "playTime": "1692469154951",
            "roleFlag": "1",
            "isPrize": "0",
            "amount": "0",
            "consumeCount": "0"
        }
        */
    }
}
