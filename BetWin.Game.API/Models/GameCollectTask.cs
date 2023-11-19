using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// 订单采集任务
    /// </summary>
    public class GameCollectTask
    {
        /// <summary>
        /// 所属的游戏编号
        /// </summary>
        public int gameId { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameType type { get; set; }

        /// <summary>
        /// 参数配置
        /// </summary>
        public string setting { get; set; }

        /// <summary>
        /// 标记时间点
        /// </summary>
        public long markTime { get; set; }
    }
}
