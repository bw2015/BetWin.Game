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

        public string PlayerName { get; set; }

        public string Password { get; set; }
    }
}
