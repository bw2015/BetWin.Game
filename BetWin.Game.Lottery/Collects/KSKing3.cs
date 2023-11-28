using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("快手三国乱斗")]
    internal class KSKing3 : CollectProviderBase
    {
        public string gateway { get; set; } = "https://tk-ks.tiantuokj.com/sgld/game/info";

        public string token { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvcGVuSWQiOiJmMTg1NTgzYzM3OThkYzAxMTRiZjU3MjQ0ODFkNzNkOSIsInZlcnNpb24iOiIxLjAuMCIsIndkenkiOmZhbHNlLCJ3ZHp5djQiOmZhbHNlLCJTR1daIjpmYWxzZSwiZXhwIjoxNzAxNzYwMDI5LCJpc3MiOiJ6enR0Iiwic3ViIjoic2dsZCJ9.lrMP5BzibBwmQT852RJDoje3c-B07ps_tBP-Ep3_9to";

        public KSKing3(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        public override IEnumerable<CollectData> Execute()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse result = client.Get(this.gateway, new Dictionary<string, string>()
                {
                    {"User-Agent","com_kwai_gif/11.10.20 (iPhone; iOS 17.1.1; Scale/3.00)" },
                    {"x-app-version","1.2.7" },
                    {"x-access-token",this.token }
                });
                if (!result) yield break;

                response? response = JsonConvert.DeserializeObject<response>(result);
                if (response == null || response.code != 0 || response.data == null || response.data.winners == null) yield break;
                yield return new CollectData(response.data.roundId.ToString(), response.data.winners.FirstOrDefault().ToString(), response.data.roundId * 1000L);

                long nextIndex = response.data.roundId + 50L;
                this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(nextIndex.ToString(), nextIndex * 1000L, response.data.roundId * 1000L));
            }
        }

        class response
        {
            public int code { get; set; }

            public responseData? data { get; set; }
        }

        class responseData
        {
            public long roundId { get; set; }

            public int expires { get; set; }

            public int[]? winners { get; set; }
        }
    }
}
