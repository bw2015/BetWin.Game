using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// API请求日志
    /// </summary>
    public struct GameAPILogModel
    {
        /// <summary>
        /// 所属游戏
        /// </summary>
        public int GameID { get; internal set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// 动作方法
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// 请求网关
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        public string Request { get; internal set; }

        /// <summary>
        /// 回应内容
        /// </summary>
        public string Response { get; internal set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public long CreateAt { get; internal set; }

        /// <summary>
        /// 执行耗时（毫秒）
        /// </summary>
        public int Time { get; internal set; }

        /// <summary>
        /// 状态代码
        /// </summary>
        public GameResultCode Status { get; internal set; }
    }
}
