using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Enums
{
    /// <summary>
    /// 语种
    /// </summary>
    public enum Language : byte
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [Description("简体中文")]
        CHN = 0,
        /// <summary>
        /// 英文
        /// </summary>
        [Description("English")]
        ENG = 2,
        /// <summary>
        /// 越南语
        /// </summary>
        [Description("Tiếng việt")]
        VI = 5,
        /// <summary>
        /// 泰语
        /// </summary>
        [Description("ไทย")]
        TH = 6,
        /// <summary>
        /// 印尼语
        /// </summary>
        [Description("indonesia")]
        IND = 13,
        /// <summary>
        /// 印地语
        /// </summary>
        [Description("हिन्दी")]
        HI = 24
    }
}
