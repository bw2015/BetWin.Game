using System.ComponentModel;

namespace BetWin.Game.API.Enums
{
    public enum GameAPIOddsType : byte
    {
        [Description("欧洲")]
        EU = 1,
        [Description("香港")]
        HK = 2,
        [Description("马来盘")]
        MY = 3,
        [Description("印尼盘")]
        IN = 4,
        [Description("美式盘")]
        AM = 5
    }
}
