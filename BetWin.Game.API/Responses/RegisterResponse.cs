using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    public class RegisterResponse : GameResponseBase
    {
        public RegisterResponse(GameResultCode code) : base(code)
        {
        }

        /// <summary>
        /// 注册成功的玩家用户名
        /// </summary>
        public string PlayerName { get; internal set; }

        /// <summary>
        /// 密码（如果有的话）
        /// </summary>
        public string Password { get; set; }

        public static implicit operator bool(RegisterResponse response)
        {
            return response.Code == GameResultCode.Success;
        }
    }
}
