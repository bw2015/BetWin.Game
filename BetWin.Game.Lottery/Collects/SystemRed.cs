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
    [Description("小红帽(系统)")]
    internal class SystemRed : SystemProviderBase
    {
        public SystemRed(string setting) : base(setting)
        {
        }

        public override LotteryType Type => LotteryType.Smart;

        protected override Dictionary<string, decimal> DefaultOdds => new Dictionary<string, decimal>()
        {
            {"1",2M },
            {"2",2M },
            {"3",32M },
            {"4",12M },
            {"5",4M }
        };

        private DateTime startTime = DateTime.Parse("2023-12-19 02:06:35");

        public override IEnumerable<CollectData> Execute()
        {
            //#1 获取当前应该开奖的期号
            long now = WebAgent.GetTimestamps() / 1000L;
            int times = (int)Math.Floor((DateTime.Now - this.startTime).TotalSeconds / 55);
            DateTime openTime = startTime.AddSeconds(times * 55);
            string index = openTime.ToString("yyyyMMddHHmmss");

            //#2 判断当前是否已经开过奖
            string? openNumber = this.handler?.GetOpenNumber(this.lotteryCode, index);

            if (string.IsNullOrEmpty(openNumber))
            {
                openNumber = this.CreateNumber(this.handler?.GetOrderResult(this.lotteryCode), out List<string> logs);

                this.handler?.SaveOpenNumber(this.lotteryCode, index, openNumber);

                DateTime nextTime = openTime.AddSeconds(55);
                string nextIndex = nextTime.ToString("yyyyMMddHHmmss");
                this.handler?.SaveIndexTime(this.lotteryCode,
                    new StepTimeModel(nextIndex, WebAgent.GetTimestamps(nextTime), WebAgent.GetTimestamps(openTime), WebAgent.GetTimestamps(openTime.AddSeconds(45)))
                    );
            }
            return new[]
            {
                 new CollectData(index,openNumber,WebAgent.GetTimestamps(openTime))
            };
        }
    }
}
