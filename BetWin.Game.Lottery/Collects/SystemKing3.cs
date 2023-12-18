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
    [Description("三国乱斗(系统)")]
    internal class SystemKing3 : SystemProviderBase
    {
        public SystemKing3(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        protected override Dictionary<string, decimal> DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",1.5M },
            {"2",2.5M },
            {"3",3.5M },
            {"4",15M },
            {"5",35M }
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
