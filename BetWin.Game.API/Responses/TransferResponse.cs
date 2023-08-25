using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Responses
{
    public class TransferResponse : GameResponseBase
    {
        public TransferResponse(GameResultCode code) : base(code)
        {
        }

        /// <summary>
        /// 本地订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 可用来查询转账订单的流水号
        /// </summary>
        public string TransferID { get; set; }

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 实际转账的金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 转账之后的余额（部分线路支持）
        /// </summary>
        public decimal? Balance { get; set; }

        public CurrencyType? Currency { get; set; }

        /// <summary>
        /// 转账后的状态
        /// </summary>
        public GameAPITransferStatus Status { get; set; }
    }
}
