using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Enums
{
    /// <summary>
    /// 提现的状态
    /// </summary>
    public enum WithdrawProviderStatus : byte
    {
        None = 0,
        /// <summary>
        /// 提交成功
        /// </summary>
        SubmitSuccess = 1,
        /// <summary>
        /// 提交失败
        /// </summary>
        SubmitFaild = 2,

        /// <summary>
        /// 付款成功（订单完成）
        /// </summary>
        PaymentSuccess = 3,
        /// <summary>
        /// 付款失败（订单完成，来自回调通知）
        /// </summary>
        PaymentFaild = 4
    }
}
