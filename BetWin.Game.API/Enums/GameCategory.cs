using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Enums
{
    public enum GameCategory : byte
    {
        [Description("彩票")]
        Lottery = 1,
        [Description("电竞")]
        ESport = 2,
        [Description("电子")]
        Slot = 3,
        [Description("真人")]
        Live = 4,
        [Description("棋牌")]
        Chess = 5,
        [Description("体育")]
        Sport = 6,
        [Description("捕鱼")]
        Fish = 7,
        [Description("小游戏")]
        Smart = 8
    }
}
