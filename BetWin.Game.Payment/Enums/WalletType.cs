using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Enums
{
    /// <summary>
    /// 本地的钱包类型
    /// </summary>
    public enum WalletType : byte
    {
        /// <summary>
        /// 银行卡
        /// </summary>
        [Description("银行卡")]
        Bank = 1,
        /// <summary>
        /// USDT - TRC20 波场协议
        /// </summary>
        [Description("数字货币")]
        Chain = 2,
        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        Alipay = 3,
        /// <summary>
        /// OKPay
        /// </summary>
        [Description("OKPay")]
        OKPay = 4
    }
}
