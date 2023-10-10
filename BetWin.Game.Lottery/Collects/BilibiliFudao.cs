using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("浮岛秘语")]
    public class BilibiliFudao : CollectProviderBase
    {
        [Description("采集地址")]
        public string Url { get; set; } = "https://api-live-bo.biliapi.net/xlive/fuxi-interface/ExploreController/getExploreLog?_ts_rpc_args_=%5B30805134%2C%223546383095171461%22%5D&access_key=b2732f2ce3fbbb7175996c304f9b5591CjDE3dn7-ffHLeckc3cg3SXePb72AqxI45Crx_15__e1OEoxLHfm4biaxHxtAQ0VcMwSVjM3RGtlYmF0SDhJamRkcXNfX21NRGNHR2JkTk04Mk9TVmFpalg4R29jb2xwM09CbE5sT1VqQnVUeExJMGpCcjk3cmg3cWl2TFFXQnlaSnRmNjVkM2l3IIEC&actionKey=appkey&appkey=1d8b6e7d45233436&build=7450400&c_locale=zh_CN&channel=xqxz_h5&device=android&disable_rcmd=0&mobi_app=android&platform=android&s_locale=zh_CN&statistics=%7B%22appId%22%3A1%2C%22platform%22%3A3%2C%22version%22%3A%227.45.0%22%2C%22abtest%22%3A%22%22%7D&ts=1695559739&sign=1166391a4871cb6c631fb917feb77ed0";

        public BilibiliFudao(string setting) : base(setting)
        {
        }

        private HttpClient client = new HttpClient();

        public override IEnumerable<CollectData> Execute()
        {
            string result = client.Get(this.Url, new Dictionary<string, string>()
            {
                {"User-Agent","Mozilla/5.0 BiliDroid/7.45.0 (bbcallen@gmail.com) os/android model/PGJM10 mobi_app/android build/7450400 channel/xqxz_h5 innerVer/7450410 osVer/12 network/2" }
            });

            JObject info = JObject.Parse(result);
            JArray? logList = info?["_ts_rpc_return_"]?["data"]?["logList"]?.Value<JArray>();
            if (logList == null) yield break;

            foreach (JToken item in logList)
            {
                long? time = item["roundTime"]?.Value<long>();
                if (time == null) continue;

                time *= 1000L;
                DateTime openTime = WebAgent.GetTimestamps(time.Value);
                string? number = item["poolId"]?.Value<string>();
                if (string.IsNullOrEmpty(number)) continue;

                yield return new CollectData(openTime.ToString("yyyyMMddHHmm"), number, time.Value + 60 * 1000);
            }
        }
    }
}
