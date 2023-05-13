using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Requests
{
    public abstract class GameModelBase
    {
        public GameModelBase(int siteId)
        {
            this.SiteID = siteId;
        }

        /// <summary>
        /// 商户号（部分线路支持站点标识）
        /// </summary>
        public int SiteID { get; private set; }
    }
}
