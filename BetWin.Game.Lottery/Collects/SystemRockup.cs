using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
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
        public override LotteryType Type => LotteryType.Smart;

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

        private static string _lastIndex = "";

        public override IEnumerable<CollectData> Execute()
        {
            //# 得到当前需要开奖的期号
            string index = DateTime.Now.AddMinutes(-1).ToString("yyyyMMddHHmm");

            //#2 判断是否已经开过
            if (_lastIndex == index) yield break;

            //# 获取当前期的投注数据
            BetOrderResult? orderResult = this.handler?.GetOrderResult(this.lotteryCode);

            string number = this.CreateNumber(orderResult, out List<string> logs);

            Console.WriteLine($"随机号码 => {number}");
            yield return new CollectData()
            {
                Index = index,
                Number = number,
                OpenTime = WebAgent.GetTimestamps()
            };

            DateTime startTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);

            // 当前的可投注期号
            string betIndex = DateTime.Now.ToString("yyyyMMddHHmm");
            long start = WebAgent.GetTimestamps(startTime),
             endTime = WebAgent.GetTimestamps(startTime.AddMinutes(1).AddSeconds(-15)),
             openTime = WebAgent.GetTimestamps(startTime.AddMinutes(1));

            this.handler?.SaveIndexTime(this.lotteryCode, new StepTimeModel(betIndex, openTime, start, endTime));

            _lastIndex = index;
        }
    }
}
