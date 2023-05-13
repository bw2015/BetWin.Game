using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    /// <summary>
    /// 退出登录
    /// </summary>
    public class LogoutResponse : GameResponseBase
    {
        public LogoutResponse(GameResultCode code) : base(code)
        {
        }

        /// <summary>
        /// 退出登录的玩家账户名
        /// </summary>
        public string PlayerName { get; internal set; }
    }
}
