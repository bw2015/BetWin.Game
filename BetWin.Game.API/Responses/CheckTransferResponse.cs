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

        public string TransferID { get; internal set; }
        public GameAPITransferStatus Status { get; internal set; }

        /// <summary>
        /// 币种
        /// </summary>
        public CurrencyType? Currency { get; internal set; }

        /// <summary>
        /// 转账的金额（如果接口有返回的话）
        /// </summary>
        public decimal? Money { get; internal set; }
    }
}
