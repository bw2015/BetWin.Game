using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    public class CheckTransferResponse : GameResponseBase
    {
        public CheckTransferResponse(GameResultCode code) : base(code)
        {
        }

        public decimal? Money { get; internal set; }
        public string TransferID { get; internal set; }
        public GameAPITransferStatus Status { get; internal set; }
    }
}
