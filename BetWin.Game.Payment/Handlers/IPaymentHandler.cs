using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Handlers
{
    public interface IPaymentHandler
    {
        /// <summary>
        /// 汇率转换
        /// </summary>
        /// <param name="amount">当前币种的金额</param>
        /// <param name="from">当前币种</param>
        /// <param name="to">要转换到的币种</param>
        /// <returns>转换之后的币值</returns>
        public decimal Conversion(decimal amount, PaymentCurrency from, PaymentCurrency to);


        public string GetProviderCode(int providerId);

        /// <summary>
        /// 锁定区块链支付的订单金额，同时写入订单信息
        /// </summary>
        /// <param name="address">收款的钱包地址</param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool LockChainOrder(string address, PaymentRequest request);

    }
}
