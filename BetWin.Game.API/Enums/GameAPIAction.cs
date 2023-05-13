using System.ComponentModel;

namespace BetWin.Game.API.Enums
{
    public enum GameAPIAction : byte
    {
        [Description("转入")]
        IN = 1,
        [Description("转出")]
        OUT = 2,
    }
}
