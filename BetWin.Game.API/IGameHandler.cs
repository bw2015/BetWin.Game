using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API
{
    /// <summary>
    /// IOC注入的接口内容
    /// </summary>
    public interface IGameAPIHandler
    {
        /// <summary>
        /// 保存游戏请求内容
        /// </summary>
        void SaveLog(GameType type, GameRequest request, GameResponse response);
    }
}
