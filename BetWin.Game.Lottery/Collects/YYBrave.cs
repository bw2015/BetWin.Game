using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("勇者之路")]
    internal class YYBrave : CollectProviderBase
    {

        public override LotteryType Type => LotteryType.Smart;
        public string url { get; set; } = "https://ysapi.yy.com/api/public/pointsoldiersyyp/queryRecentResultInfo.json?data=%7B%22extendInfo%22:%7B%7D,%22mode%22:5%7D&ticket=boIB7zCCAeugAwIBBaEDAgEOogcDBQAAAAAAo4IBXGGCAVgwggFUoAMCAQWhERsPeXkuY29tAAAEAAEKQ7CAohEwD6ADAgEBoQgwBhsENTA2MKOCASUwggEhoAQCAgEVoQMCAQKiggESBIIBDggCAKCJfwAACAIAoIl%252FAABmQHrVxRNSiPJKkcq1gStM4vGhsQtPUSysRwnm2yRiyBWiTDh9w7RvIHEwjNO1LG8y7GsA%252BDam82mQ7cNFuOR8HmmNuhROKCzowZtfnq9i7JWMZ861R3OOzKiXDIpMrC2AR1CJUUaPeug03bwEUKY7ZAjZEO1CeOj8eatGOz25hSYewTp3fpod%252F%252FxRHJINkaxn5zY5oMzGdQzCz0YEzZH4c9e9hmXUWJ2vh0MabwEUtbzRt2M7orpTpViIivFjR90RNqeHiuHLdI9rPvoL%252BT9P1T2jD7ntc9%252BH2pEczL2CQ2G%252FJZEJqxFFHB4UF8yBBgX7L9vCprMNfoUS0iCagqR2MHSgAwIBEaJtBGsccm7cxdMAYEUdIE6Z5TaGo4RlHHbI6Zlm5lm9VsVA0RReZcdJIYu0%252F14ZnhEGiiMt%252B1FJELOjFXnzhrsUv3%252F3yqNiAz7WBXRNjbfdSGtG8Mrxcttv90w8wLBh8ZdsB3pKI1BHDVTKI8H%252B3A%253D%253D";

        public YYBrave(string setting) : base(setting)
        {
        }

        string getNumber(string? winName)
        {
            return winName switch
            {
                "战士" => "1",
                "弓箭手" => "2",
                "骑士" => "3",
                "守护者" => "4",
                "超级天使" => "5",
                _ => "0"
            };
        }

        public override IEnumerable<CollectData> Execute()
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                HttpClientResponse result = client.Get(url, new Dictionary<string, string>()
                {

                });
                if (!result) yield break;

                response? res = JsonConvert.DeserializeObject<response>(result.Content);
                foreach (var item in res?.wininfo ?? Array.Empty<winner>())
                {
                    yield return new CollectData()
                    {
                        Index = item.bizTime.ToString("yyyyMMddHHmm"),
                        Number = this.getNumber(item.winName),
                        OpenTime = WebAgent.GetTimestamps(item.bizTime)
                    };
                }
            }
        }

        class response
        {
            public int result { get; set; }

            public winner[]? wininfo { get; set; }
        }

        class winner
        {
            public string? winName { get; set; }

            public DateTime bizTime { get; set; }
        }
    }
}
