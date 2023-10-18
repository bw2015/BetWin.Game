using BetWin.Game.API.Enums;
using BetWin.Game.API.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public struct GameRequest
    {
        /// <summary>
        /// 动作
        /// </summary>
        public APIMethod Method;

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url;

        /// <summary>
        /// 请求数据
        /// </summary>
        public string Data;

        /// <summary>
        /// 请求的参数
        /// </summary>
        public HttpClientOption Option;

       
    }
}
