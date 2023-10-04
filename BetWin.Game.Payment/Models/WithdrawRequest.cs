using BetWin.Game.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Models
{
    /// <summary>
    /// 提现的请求内容
    /// </summary>
    public class WithdrawRequest
    {
        /// <summary>
        /// 本地的订单编号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 发起提现的用户名
        /// </summary>
        public string? userName { get; set; }

        /// <summary>
        /// 钱包类型
        /// </summary>
        public WithdrawWalletType? wallet { get; set; }

        /// <summary>
        /// 银行简码
        /// </summary>
        public PaymentBankCode? bankCode { get; set; }

        /// <summary>
        /// 收款人的姓名（适用于银行打款）
        /// </summary>
        public string? bankName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string? account { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public PaymentCurrency currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal amount { get; set; }
    }
}
