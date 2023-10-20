using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    [Description("火箭升空(系统)")]
    public class SystemRockup : SystemProviderBase
    {
        public SystemRockup(string setting) : base(setting)
        {
        }

        /// <summary>
        /// 水星
        /// </summary>
        const string Mercury = "6";

        /// <summary>
        /// 金星
        /// </summary>
        const string Venus = "4";

        /// <summary>
        /// 地球
        /// </summary>
        const string Earth = "3";

        /// <summary>
        /// 火星
        /// </summary>
        const string Mars = "5";

        /// <summary>
        /// 木星
        /// </summary>
        const string Jupiter = "8";

        /// <summary>
        /// 土星
        /// </summary>
        const string Saturn = "1";

        /// <summary>
        /// 天王星
        /// </summary>
        const string Uranus = "2";

        /// <summary>
        /// 海王星
        /// </summary>
        const string Neptune = "7";

        protected override Dictionary<string, decimal> DefaultOdds => new Dictionary<string, decimal>()
            {
                { Mercury,15 },
                { Venus,25 },
                { Earth,5 },
                { Mars,5 },
                { Jupiter,15 },
                { Saturn,5 },
                { Uranus,25 },
                { Neptune,5 }
            };

        public override IEnumerable<CollectData> Execute()
        {
            //# 得到当前的时间点
            string index = DateTime.Now.AddMinutes(-1).ToString("yyyyMMddHHmm");

            //# 判断是否已经开过
            string number = this.CreateNumber();
            Console.WriteLine($"随机号码 => {number}");
            yield return new CollectData()
            {
                Index = index,
                Number = number,
                OpenTime = WebAgent.GetTimestamps()
            };

            DateTime startTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);
            this.handler?.SaveStepTime(this.lotteryCode ?? string.Empty, new StepTimeModel()
            {
                index = DateTime.Now.ToString("yyyyMMddHHmm"),
                startTime = WebAgent.GetTimestamps(startTime),
                endTime = WebAgent.GetTimestamps(startTime.AddSeconds(50)),
                openTime = WebAgent.GetTimestamps(startTime.AddMinutes(1))
            });
        }
    }
}
