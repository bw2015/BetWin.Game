using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Withdraws
{
    [Description("手动出款")]
    public class None : WithdrawProviderBase
    {
        public None(string setting) : base(setting)
        {
        }

        public override WithdrawResponse Withdraw(WithdrawRequest request)
        {
            return new WithdrawResponse()
            {
                orderId = request.orderId,
                currency = request.currency,
                amount = request.amount,
                status = WithdrawProviderStatus.Success
            };
        }

        public override WithdrawQueryResponse Query(WithdrawQuery request)
        {
            return new WithdrawQueryResponse()
            {
                OrderID = request.orderId,
                TradeNo = request.tradeNo,
                Status = WithdrawProviderStatus.Success
            };
        }
    }
}
