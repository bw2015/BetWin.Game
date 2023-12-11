using BetWin.Game.API.Enums;
using BetWin.Game.API.Providers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BetWin.Game.API
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameFactory
    {
        /// <summary>
        /// 获取游戏对象
        /// </summary>
        public static IGameProvider? GetGame(GameType type, string setting)
        {
            Type handlerType = Assembly.GetAssembly(typeof(GameFactory)).GetType($"{typeof(IGameProvider).Namespace}.{type}");
            if (handlerType == null) return null;
            return (IGameProvider)Activator.CreateInstance(handlerType, new[] { setting });
        }

        /// <summary>
        /// 获取采集的配置信息
        /// </summary>
        public static IGameCollect? GetCollect(GameType type)
        {
            Type handlerType = Assembly.GetAssembly(typeof(GameFactory)).GetType($"{typeof(IGameCollect).Namespace}.{type}");
            if (handlerType == null) return null;
            return (IGameCollect)Activator.CreateInstance(handlerType, new[] { "{}" });
        }
    }
}
