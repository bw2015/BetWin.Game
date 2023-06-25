using BetWin.Game.Lottery.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Attributes
{
    /// <summary>
    /// 玩法的属性说明
    /// </summary>
    public class PlayAttribute : Attribute
    {
        /// <summary>
        /// 玩法的多语种名称
        /// </summary>
        public Dictionary<Language, string> Name { get; set; } = new Dictionary<Language, string>();

        /// <summary>
        /// 玩法的多语种名称配置
        /// </summary>
        /// <param name="chinese"></param>
        /// <param name="english"></param>
        /// <param name="vietnamese"></param>
        public PlayAttribute(string chinese, string english, string vietnamese, string? thai = null)
        {

        }
    }
}
