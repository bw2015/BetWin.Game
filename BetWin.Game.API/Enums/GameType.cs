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
        [Description("BBIN真人"), Game(Category = GameCategory.Live)]
        BBINLive,
        [Description("EBet真人"), Game(Category = GameCategory.Live)]
        EBetLive,

        [Description("VR彩票"), Game(Category = GameCategory.Lottery)]
        VRLottery = 20,
        [Description("OB彩票"), Game(Category = GameCategory.Lottery)]
        OBLottery,
        [Description("BBIN彩票"), Game(Category = GameCategory.Lottery)]
        BBLottery,
        [Description("双赢彩票"), Game(Category = GameCategory.Lottery)]
        SGLottery,

        [Description("OB电竞"), Game(Category = GameCategory.ESport)]
        OBESport = 40,
        [Description("雷火电竞"), Game(Category = GameCategory.ESport)]
        TFESport,

        [Description("开元棋牌"), Game(Category = GameCategory.Chess)]
        KYChess = 60,
        [Description("高登棋牌"), Game(Category = GameCategory.Chess)]
        GDChess,
        [Description("乐游棋牌"), Game(Category = GameCategory.Chess)]
        LYChess,
        [Description("V8棋牌"), Game(Category = GameCategory.Chess)]
        V8Chess,
        [Description("双赢棋牌"), Game(Category = GameCategory.Chess)]
        WWChess,
        [Description("云游棋牌"), Game(Category = GameCategory.Chess)]
        YYChess,

        [Description("OB体育"), Game(Category = GameCategory.Sport)]
        OBSport = 80,
        [Description("平博体育"), Game(Category = GameCategory.Sport)]
        PBSport,
        [Description("BB体育"), Game(Category = GameCategory.Sport)]
        BBSport,
        [Description("FB体育"), Game(Category = GameCategory.Sport)]
        FBSport,
        [Description("沙巴体育"), Game(Category = GameCategory.Sport)]
        SBSport,

        [Description("BBIN电子"), Game(Category = GameCategory.Slot)]
        BBSlot = 100,
        [Description("高登电子"), Game(Category = GameCategory.Slot)]
        GDSlot,
        [Description("MG电子"), Game(Category = GameCategory.Slot)]
        MGSlot,
        [Description("OB电子"), Game(Category = GameCategory.Slot)]
        OBSlot,
        [Description("PG电子"), Game(Category = GameCategory.Slot)]
        PGSlot,
        [Description("PP电子"), Game(Category = GameCategory.Slot)]
        PPSlot,
        [Description("PT电子"), Game(Category = GameCategory.Slot)]
        PTSlot
    }

    public class GameAttribute : Attribute
    {
        public GameCategory Category { get; set; }
    }
}
