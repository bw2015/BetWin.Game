using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    public class BalanceResponse : GameResponseBase
    {
        public BalanceResponse(GameResultCode code) : base(code)
        {
        }

        public decimal Balance { get; set; }

        public static implicit operator bool(BalanceResponse response)
        {
            return response.Code == GameResultCode.Success;
        }
    }
}
