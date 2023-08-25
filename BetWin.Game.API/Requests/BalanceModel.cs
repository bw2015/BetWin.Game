using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Requests
{
    public class BalanceModel : GameModelBase
    {
        public BalanceModel(int siteId) : base(siteId)
        {
        }

        /// <summary>
        /// 用户的币种
        /// </summary>
        public CurrencyType? Currency{ get; set; }

        public string PlayerName { get; set; }

        public string? Password { get; set; }
    }
}
