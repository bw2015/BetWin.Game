using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Enums
{
    public enum GameType : byte
    {
        [Description("EVO真人"), Game(Category = GameCategory.Live)]
        EVOLive = 1,
        [Description("SA真人"), Game(Category = GameCategory.Live)]
        SALive,
        [Description("OB真人"), Game(Category = GameCategory.Live)]
        OBLive,

        [Description("VR彩票"), Game(Category = GameCategory.Lottery)]
        VRLottery = 20,
        [Description("OB彩票"), Game(Category = GameCategory.Lottery)]
        OBLottery = 21,

        [Description("OB电竞"), Game(Category = GameCategory.ESport)]
        OBESport = 40,
        [Description("雷火电竞"), Game(Category = GameCategory.ESport)]
        TFESport = 41,

        [Description("开元棋牌"), Game(Category = GameCategory.Chess)]
        KYChess = 60
    }

    public class GameAttribute : Attribute
    {
        public GameCategory Category { get; set; }
    }
}
