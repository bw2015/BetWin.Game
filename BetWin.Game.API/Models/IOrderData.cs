using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// 订单数据的扩展
    /// </summary>
    public interface IOrderData
    {
        /// <summary>
        /// 类型
        /// </summary>
        string Type { get; }
    }
}
