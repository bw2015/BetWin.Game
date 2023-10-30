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
    [Description("YY 纵横战场")]
    internal class YYBattle : CollectProviderBase
    {
        public string url { get; set; } = "https://ysapi.yy.com/api/public/pointsoldiersyyp/queryRecentResultInfo.json?data=%7B%22extendInfo%22:%7B%7D,%22mode%22:0%7D&ticket=boIB7zCCAeugAwIBBaEDAgEOogcDBQAAAAAAo4IBXGGCAVgwggFUoAMCAQWhERsPeXkuY29tAAAEAAEKQ7CAohEwD6ADAgEBoQgwBhsENTA2MKOCASUwggEhoAQCAgEVoQMCAQKiggESBIIBDggCAMDNfwAACAIAwM1%252FAABmQHrVxRNSiPJKkcq1gStM4vGhsQtPUSysRwnmoe5kZuChQckae4G1cSj7iJy1LG8y7GsA%252BDam82mQ7cNFuOR8HmmNuhROKCzowZtfnq9i7JWMZ861R3OOzKiXDIpMrC2AR1CJUUaPhNmFLL6TLHjbZAjZEO1CeOj8qPGcByyaEcoY4zp3fpod%252F%252FxRHJINkaxn5zY5oMzGdQzCz0YEzZH4c9e9hmXUWJ2vh0MabwEUtbzRt2M7orpTpViIivFjR90RNqeHiuHLdI9rPvoL%252BT8e%252BZdn4POFS2%252BmulVAAroCkBWu8bMJqxFFHB4UF8yBBgX7L7lXOtciJVnAFe6lIKR2MHSgAwIBEaJtBGtCipyG4IwpMLkX1xBiMvnJ5BlltKpDVKxd%252BDj5zpbGHam9gEbSVU2U7TKMGxLqv2ROb%252FNBFJIYQj6ZN3AgQYApZJtCdnMbL57TNr%252F9U06qRlm9gxz46BGZb4zPl54yVpSe8VvkheRSHF%252Bd9Q%253D%253D";

        public YYBattle(string setting) : base(setting)
        {
        }

        string getNumber(string? winName)
        {
            return winName switch
            {
                "战士" => "1",
                "弓箭手" => "2",
                "骑士" => "3",
                "法师" => "4",
                "魔导师" => "5",
                "守护者" => "6",
                "大天使" => "7",
                _ => "0"
            };
        }

        public override LotteryType Type => LotteryType.Smart;

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
