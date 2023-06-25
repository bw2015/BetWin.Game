using BetWin.Game.Lottery.Base;
using BetWin.Game.Lottery.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BetWin.Game.Lottery
{
    public static class LotteryFactory
    {
        /// <summary>
        /// 得到类库中的所有玩法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<ILotteryPlay> GetPlays(LotteryType type)
        {
            Assembly assembly = typeof(LotteryFactory).Assembly;
            string playNamespace = $"{typeof(LotteryFactory).Namespace}.{type}.";
            foreach (Type playType in assembly.GetTypes().Where(t => t.FullName.StartsWith(playNamespace) && !t.IsAbstract))
            {
                yield return (ILotteryPlay)Activator.CreateInstance(playType);
            }
        }
    }
}
