using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// API请求的返回值
    /// </summary>
    public struct GameResponse
    {
        public GameResponse(string content, GameResultCode code, string message, int time, GameRequest? request = null)
        {
            this.Content = content;
            this.Code = code;
            this.Message = message;
            this.Time = time;
            this.Request = request;
        }

        /// <summary>
        /// 请求返回值
        /// </summary>
        public string Content;

        public GameResultCode Code;

        /// <summary>
        /// 如果发生错误，错误的内容
        /// </summary>
        public string Message;

        /// <summary>
        /// 时长（毫秒）
        /// </summary>
        public int Time;

        /// <summary>
        /// 请求参数
        /// </summary>
        public GameRequest? Request;

        public static implicit operator string(GameResponse response)
        {
            return response.Content;
        }

        public static implicit operator GameResultCode(GameResponse response)
        {
            return response.Code;
        }

        public static implicit operator bool(GameResponse response)
        {
            return response.Code == GameResultCode.Success;
        }
    }
}
