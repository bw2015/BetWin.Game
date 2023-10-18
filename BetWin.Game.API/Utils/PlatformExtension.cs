using BetWin.Game.API.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Utils
{
    internal static class PlatformExtension
    {
        public static bool IsMobile(this GamePlatform platform)
        {
            return platform == GamePlatform.Mobile;
        }
    }
}
