using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Logs
{
    /// <summary>
    /// 请求充值的日志信息
    /// </summary>
    public struct PaymentLog
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 供应商的类型
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// 返回内容
        /// </summary>
        public string Response { get; set; }
    }
}
