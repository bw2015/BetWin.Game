using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Collects
{
    /// <summary>
    /// 系统开奖基类
    /// </summary>
    public abstract class SystemProviderBase : CollectProviderBase
    {
        protected SystemProviderBase(string setting) : base(setting)
        {
        }

        /// <summary>
        /// 默认赔率
        /// </summary>
        protected abstract Dictionary<string, decimal> DefaultOdds { get; }

        /// <summary>
        /// 根据默认赔率得出随机的号码
        /// </summary>
        /// <returns></returns>
        protected virtual string CreateNumber()
        {
            decimal total = DefaultOdds.Sum(t => t.Value);

            // 所有的开奖号码
            string[] numbers = this.DefaultOdds.Select(t => t.Key).ToArray();

            // 离散列表
            decimal[] results = this.DefaultOdds.Select(t => total / t.Value).ToArray();

            // 权重数列
            Tuple<string, decimal>[] weights = new Tuple<string, decimal>[results.Length];
            decimal p = decimal.Zero;
            for (int i = 0; i < results.Length; i++)
            {
                p += results[i];
                weights[i] = new Tuple<string, decimal>(numbers[i], p);
            }

            int random = new Random().Next((int)total);
            string number = weights.First(t => random < t.Item2).Item1;

            return number;
        }
    }
}
