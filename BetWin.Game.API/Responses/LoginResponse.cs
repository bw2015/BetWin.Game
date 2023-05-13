using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    /// <summary>
    /// 登录返回
    /// </summary>
    public class LoginResponse : GameResponseBase
    {
        public LoginResponse(GameResultCode code) : base(code)
        {
        }

        public string Url { get; set; }

        public LoginMethod Method { get; set; }

        /// <summary>
        /// 如果登录方式是Post需要携带的数据
        /// </summary>
        public Dictionary<string, object> Data { get; set; }
    }
}
