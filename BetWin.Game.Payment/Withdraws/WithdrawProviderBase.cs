using BetWin.Game.Payment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Withdraws
{
    /// <summary>
    /// 提现供应商的基类
    /// </summary>
    public abstract class WithdrawProviderBase : IWithdrawProvider
    {
        public WithdrawProviderBase(string setting)
        {
            if (string.IsNullOrEmpty(setting) || !setting.StartsWith("{")) return;
            JsonConvert.PopulateObject(setting, this);
        }

        public abstract WithdrawResponse Query(WithdrawRequest request);

        public abstract WithdrawResponse Withdraw(WithdrawRequest request);

    }
}
