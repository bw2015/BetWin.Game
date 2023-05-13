using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Enums
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum GameAPIOrderStatus
    {
        /// <summary>
        /// 等待开奖
        /// </summary>
        Wait = 0,
        /// <summary>
        /// 赢
        /// </summary>
        Win = 1,
        /// <summary>
        /// 输
        /// </summary>
        Lose = 2,
        /// <summary>
        /// 退款
        /// </summary>
        Return = 3
    }
}
