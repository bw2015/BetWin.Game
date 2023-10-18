using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    public abstract class GameResponseBase
    {
        public GameResponseBase(GameResultCode code)
        {
            this.Code = code;
        }

        /// <summary>
        /// 返回的状态编码（只可在构造中赋值）
        /// </summary>
        public GameResultCode Code { get; private set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string? Message { get; set; }
    }
}
