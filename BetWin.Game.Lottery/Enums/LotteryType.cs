using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Lottery.Enums
{
    /// <summary>
    /// 彩票类型
    /// </summary>
    public enum LotteryType : byte
    {
        [Description("时时彩")]
        SSC = 1,
        [Description("快3")]
        K3 = 2,
        [Description("PK10")]
        PK10 = 3,
        [Description("六合彩")]
        Mark6 = 4,
        [Description("排列3")]
        PL3 = 5,
        [Description("七星彩")]
        QXC = 6,
        [Description("越南南部彩")]
        YNCS = 7,
        [Description("幸运3")]
        Lucky3 = 8
    }
}
