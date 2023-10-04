using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Enums
{
    /// <summary>
    /// 本地的钱包类型
    /// </summary>
    public enum WithdrawWalletType
    {
        /// <summary>
        /// 银行卡
        /// </summary>
        Bank = 1,
        /// <summary>
        /// USDT - TRC20 波场协议
        /// </summary>
        USDT_TRC20 = 2
    }
}
