using BetWin.Game.Payment.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Withdraws
{
    /// <summary>
    /// 提现供应商
    /// </summary>
    public interface IWithdrawProvider
    {
        /// <summary>
        /// 提交提现请求
        /// </summary>
        public WithdrawResponse Withdraw(WithdrawRequest request);

        /// <summary>
        /// 查询提现状态
        /// </summary>
        public WithdrawQueryResponse Query(WithdrawQuery request);
    }
}
