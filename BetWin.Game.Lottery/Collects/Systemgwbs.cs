using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("怪物捕手(系统)")]
    internal class Systemgwbs : SystemProviderBase
    {
        public Systemgwbs(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        protected override Dictionary<string, decimal> DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",4.5M },
            {"2",4.5M },
            {"3",36M },
            {"4",26M },
            {"5",10M },
            {"6",10M },
            {"7",4.5M },
            {"8",4.5M },
        };

        private DateTime startTime = new DateTime(2023, 12, 19, 02, 06, 40);

        public override IEnumerable<CollectData> Execute()
        {
            int times = (int)Math.Floor((DateTime.Now - startTime).TotalSeconds / 50D);

            DateTime openTime = startTime.AddSeconds(times * 50);
            string index = (WebAgent.GetTimestamps(openTime) / 1000L).ToString();

            string? openNumber = this.handler?.GetOpenNumber(this.lotteryCode, index);
            if (string.IsNullOrEmpty(openNumber))
            {
                openNumber = this.CreateNumber(this.handler?.GetOrderResult(this.lotteryCode), out _);
                this.handler?.SaveOpenNumber(this.lotteryCode, index, openNumber);

                DateTime nextTime = openTime.AddSeconds(50);
                string nextIndex = (WebAgent.GetTimestamps(nextTime) / 1000L).ToString();
                this.handler?.SaveIndexTime(this.lotteryCode,
                    new StepTimeModel(nextIndex, WebAgent.GetTimestamps(nextTime), WebAgent.GetTimestamps(openTime))
                    );
            }

            return new[]
            {
                new CollectData(index,openNumber,WebAgent.GetTimestamps(openTime))
            };
        }
    }
}
