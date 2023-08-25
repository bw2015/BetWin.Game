using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Exceptions
{
    /// <summary>
    /// 支付发生的错误
    /// </summary>
    public enum PaymentError
    {
        [Description("未实现注入类库")]
        NoPaymentHandler = 1,
        [Description("充值金额被锁定")]
        AmountLocked = 2
    }
}
