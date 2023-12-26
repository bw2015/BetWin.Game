using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
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
        public string url { get; set; } = "https://h5api.m.taobao.com/h5/mtop.alicpbingo.alicpraceservice.listhistory/1.0/?jsv=2.4.16&appKey=12574478&t=1703432484061&sign=cc274d6755231c5f0ba86a45abe663ac&url=mtop.alicpbingo.AlicpRaceService.listHistory&api=mtop.alicpbingo.AlicpRaceService.listHistory&v=1.0&dataType=originaljsonp&type=originaljsonp&callback=mtopjsonp16&data=%7B%22accessToken%22%3A%227a636103f56f4a47339415da6421dc3f-26-1126-4476%22%7D";

        public string cookie { get; set; } = "enc=D%2FDNr0yfiNZ2rgT342dJzqxN0xgxOxTOJsZi4DREDNKlsxOUB1Wik0fc6mtXrwxCBdlhapBHdY43hYZjHmFo7UUw0fphiwGuqmJaD3sC85Y%3D; cna=K+wMGpyIEQ0CASuHXPMcGtzU; t=703ad13107f41d06edd7fe9b2e823d29; _m_h5_tk=1bf0908313fcbb0e25f4ed9ffe180122_1703439858141; _m_h5_tk_enc=5f4faeea7403b18f3ac6a34c0d9201a6; _samesite_flag_=true; cookie2=1f05f24bcaa6b273e4ebb319440c58ad; _tb_token_=feb33e568471; xlly_s=1; sgcookie=E100%2BiJ57VtFiUovel4rKgkYauyx8xlIkkH4xrdvx4hrvp8bYIukWLfhGaFXkXMiTiiq9kZ3ge0CKVfIhI0EP6G7xM6iWSUTJV40zSU7WM%2BkAjYmf%2FpLMEg6iPtOolJfRZnK; ockeqeudmj=u7XbcU0%3D; _w_tb_nick=sp84905; munb=24190546; WAPFDFDTGFG=%2B4cMKKP%2B8PI%2BL8mCV8lf6o8A; _w_app_lg=0; unb=24190546; uc3=nk2=EE5ss%2BLjZQ%3D%3D&lg2=W5iHLLyFOGW7aA%3D%3D&vt3=F8dD3Cb0KOHOa%2Flydlw%3D&id2=UUwRk6O8Ks8%3D; uc1=cookie21=V32FPkk%2Fgi8IDE6SJpp2&cookie15=UIHiLt3xD8xYTw%3D%3D&existShop=false&cookie14=UoYelq0wxyCflQ%3D%3D; csg=d7b021e3; lgc=sp84905; ntm=0; cancelledSubSites=empty; cookie17=UUwRk6O8Ks8%3D; dnk=sp84905; skt=70548c4cc053db88; uc4=nk4=0%40Epcq1m0WUIycSwfKbmFGj4GL&id4=0%40U27O%2F0Cb6Y0Du4xJYoQyrqyF2g%3D%3D; tracknick=sp84905; _cc_=W5iHLLyFfA%3D%3D; _l_g_=Ug%3D%3D; sg=565; _nk_=sp84905; cookie1=BvHUpzZtA7H3nxplmEHYWQ%2Fw9sDjHbAmOhaYbnpDDG4%3D; l=fBjMQPrRgfcqT4LxBOfZPurza779SIROguPzaNbMi9fP_Sfe5ERRW1C9BWLwCnGResEpu3WZocJJB6zXqyCSnxv9-j-UCWMmnhupLp3h.; tfstk=eZAWbc4h_uq7tPyuc0g2fliLziCBN3GZAy_pSeFzJ_C-RHt9V9WpzeQdOnKErg8KqHOCDnIy4L8ehyTMcH6nrMSdAH8pUqlZ_UYlt6IIbflNtSwIQco2OD_yr6fK0Im7ptYkbbInkMkq3PrmmUl3tWPIfH7vI28JPI_5HYLGD5Qk1a65XQR79W3OPTs9yisrf5S6Se2QKgFdlGujlJ23oA7TfwqHAQXRoZlElqwFOTQclGujlJ2heZbqTqgbL61..; isg=BAYG-v9ln5kdhk-8Vfx7VK14V_yIZ0oh-wJ3RvAv8ykE86MNWPdFME3Jzyk_20I5";

        public AliAnimal(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            List<CollectData> list = new List<CollectData>();
            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(this.url, new Dictionary<string, string>()
                {
                    {"Cookie",this.cookie },
                    {"User-Agent","Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.6 Mobile/15E148 Safari/604.1" }
                });
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
                return list;
            }
        }

        string? getContent(string result)
        {
            Regex regex = new Regex(@"mtopjsonp16\((?<Content>.+)\)");
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
