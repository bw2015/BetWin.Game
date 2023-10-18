using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Enums
{
    public enum GameType : byte
    {
        [Description("EVO真人")]
        EVOLive = 1,
        [Description("SA真人")]
        SALive,

        [Description("VR彩票")]
        VRLottery = 20,

        [Description("OB电竞")]
        OBESport = 40,
    }
}
