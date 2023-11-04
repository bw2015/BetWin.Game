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
        /// <summary>
        /// 状态未知，用于代付接口发生异常的情况
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// 代付请求提交成功（对应订单状态为付款中）
        /// </summary>
        RequestSuccess = 1,
        /// <summary>
        /// 代付订单提交失败（对应订单状态不改变）
        /// </summary>
        RequestFaild = 2,

        /// <summary>
        /// 付款成功（订单完成）
        /// </summary>
        Success = 3,
        /// <summary>
        /// 付款失败（订单完成，来自回调通知）
        /// </summary>
        Faild = 4
    }
}
