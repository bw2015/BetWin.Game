using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Requests
{
    /// <summary>
    /// 检查订单
    /// </summary>
    public class CheckTransferModel : GameModelBase
    {
        public CheckTransferModel(int siteId) : base(siteId)
        {
        }

        public string PlayerName { get; set; }

        /// <summary>
        /// 提交给上游线路查询的转账订单ID
        /// </summary>
        public string OrderID { get; set; }

        public GameAPIAction Action { get; set; }
    }
}
