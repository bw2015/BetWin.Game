using BetWin.Game.API.Enums;
using BetWin.Game.API.Handlers;
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
        public static IGameHandler? GetGame(GameType type, string setting)
        {
            Type handlerType = Assembly.GetAssembly(typeof(GameFactory)).GetType($"{typeof(GameFactory).Namespace}.Handlers.{type}");
            if (handlerType == null) return null;
            return (IGameHandler)Activator.CreateInstance(handlerType, new[] { setting });
        }
    }
}
