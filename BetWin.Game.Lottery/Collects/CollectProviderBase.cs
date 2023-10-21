using BetWin.Game.Lottery.Collects.Models;
using BetWin.Game.Lottery.Enums;
using BetWin.Game.Lottery.Handlers;
using Newtonsoft.Json;
using SP.StudioCore.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    /// <summary>
    /// 采集供应商基类
    /// </summary>
    public abstract class CollectProviderBase
    {
        /// <summary>
        /// 彩种代码
        /// </summary>
        public string lotteryCode { get; set; } = "";

        protected ILotteryHandler? handler => IocCollection.GetService<ILotteryHandler>();

        /// <summary>
        /// 参数配置
        /// </summary>
        public CollectProviderBase(string setting)
        {
            if (string.IsNullOrEmpty(setting)) return;
            JsonConvert.PopulateObject(setting, this);
        }

        /// <summary>
        /// 运行采集器
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<CollectData> Execute();

        public abstract LotteryType Type { get; }
    }
}
