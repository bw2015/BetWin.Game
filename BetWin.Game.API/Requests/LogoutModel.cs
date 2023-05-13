using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Requests
{
    /// <summary>
    /// 退出登录的请求
    /// </summary>
    public class LogoutModel : GameModelBase
    {
        public LogoutModel(int siteId) : base(siteId)
        {
        }

        public string PlayerName { get; set; }
    }
}
