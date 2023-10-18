using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Enums
{
    /// <summary>
    /// 进入游戏的设备类型
    /// </summary>
    [Flags]
    public enum GamePlatform : byte
    {
        [Description("网页端")]
        PC = 1,
        [Description("移动端")]
        Mobile = 2
    }
}
