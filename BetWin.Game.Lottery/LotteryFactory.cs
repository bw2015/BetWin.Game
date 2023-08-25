using BetWin.Game.Lottery.Base;
using BetWin.Game.Lottery.Collects;
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
            string playNamespace = $"BetWin.Game.Lottery.Plays.{type}.";
            foreach (Type playType in assembly.GetTypes().Where(t => t.FullName.StartsWith(playNamespace) && !t.IsAbstract))
            {
                yield return (ILotteryPlay)Activator.CreateInstance(playType);
            }
        }

        public static ILotteryPlay GetPlay(LotteryType type, string className)
        {
            Assembly assembly = typeof(LotteryFactory).Assembly;
            string playNamespace = $"BetWin.Game.Lottery.Plays.{type}.{className}";
            Type playType = assembly.GetType(playNamespace);
            return (ILotteryPlay)Activator.CreateInstance(playType);
        }

        /// <summary>
        /// 获取所有的采集供应商
        /// </summary>
        public static IEnumerable<Type> GetCollectProviders()
        {
            Assembly assembly = typeof(LotteryFactory).Assembly;
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CollectProviderBase)) && !t.IsAbstract);
        }

        public static CollectProviderBase? GetCollectProvider(string providerName, string setting)
        {
            Type type = GetCollectProviders().FirstOrDefault(t => t.Name == providerName);
            if (type == null) return null;
            return (CollectProviderBase)Activator.CreateInstance(type, new[] { setting });
        }
    }
}
