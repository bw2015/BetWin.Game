using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Enums
{
    /// <summary>
    /// API请求的动作
    /// </summary>
    public enum APIMethod
    {
        Login,
        Logout,
        Balance,
        Transfer,
        CheckTransfer,
        GetOrder,
        Register
    }
}
