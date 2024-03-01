using BetWin.Game.Lottery.Handlers;
using BetWin.Game.Lottery.Models;
using SP.StudioCore.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Base
{
    /// <summary>
    /// 彩票玩法的方法接口
    /// </summary>
    public interface ILotteryPlay
    {
        /// <summary>
        /// 获取投注的注数
        /// </summary>
        public int GetBetCount(string input);

        /// <summary>
        /// 获取奖金（为0表示不中奖）
        /// </summary>
        /// <param name="input">投注内容</param>
        /// <param name="openNumber">开奖号码</param>
        /// <param name="odds">赔率</param>
        /// <returns>中奖倍数</returns>
        public RewardOdds GetReward(string input, OpenNumber openNumber, Odds odds);

        /// <summary>
        /// 获取当前的默认赔率
        /// </summary>
        /// <returns></returns>
        public Odds GetDefaultOdds();

        /// <summary>
        /// 可投注的号码
        /// </summary>
        public string[] GetBetNumbers();
    }

    /// <summary>
    /// 彩票的基类
    /// </summary>
    public abstract class LotteryBase : ILotteryPlay
    {
        protected virtual ILotteryHandler? handler => IocCollection.GetService<ILotteryHandler>();

        /// <summary>
        /// 检查开奖号码是否符合规范
        /// </summary>
        protected abstract bool CheckOpenNumber(OpenNumber openNumber);

        /// <summary>
        /// 检查投注内容是否符合规范
        /// </summary>
        protected abstract bool CheckBetContent(string content);

        /// <summary>
        /// 默认的赔率
        /// </summary>
        /// <returns></returns>
        protected abstract Odds DefaultOdds { get; }

        protected virtual string[] BetNumbers => throw new NotImplementedException();



        public Odds GetDefaultOdds() => this.DefaultOdds;

        public virtual string[] GetBetNumbers() => this.GetDefaultOdds();

        /// <summary>
        /// 判断是否中奖
        /// </summary>
        protected abstract RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds);

        protected abstract int GetBetCountNumber(string input);

        /// <summary>
        /// 获取奖金（为0表示不中奖）
        /// </summary>
        /// <param name="input">投注内容</param>
        /// <param name="openNumber">开奖号码</param>
        /// <param name="odds">赔率</param>
        /// <returns>中奖倍数</returns>
        public RewardOdds GetReward(string input, OpenNumber openNumber, Odds odds)
        {
            if (!this.CheckOpenNumber(openNumber)) return decimal.Zero;
            return this.CheckReward(input, openNumber, odds);
        }

        /// <summary>
        /// 获取投注的注数
        /// </summary>
        public int GetBetCount(string input)
        {
            if (!this.CheckBetContent(input)) return 0;
            return this.GetBetCountNumber(input);
        }
    }
}
