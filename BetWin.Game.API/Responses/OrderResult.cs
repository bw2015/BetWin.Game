using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using System.Collections.Generic;

namespace BetWin.Game.API.Responses
{

    public class OrderResult : GameResponseBase
    {
        public OrderResult(GameResultCode code) : base(code)
        {
        }

        /// <summary>
        /// 订单数据
        /// </summary>
        public List<OrderData>? data { get; set; }

        /// <summary>
        /// 本次实际查询的范围
        /// </summary>
        public long? startTime { get; set; }

        public long? endTime { get; set; }

        public static implicit operator OrderResult(GameResultCode code)
        {
            return new OrderResult(code);
        }

        public static implicit operator bool(OrderResult result)
        {
            return result.Code == GameResultCode.Success;
        }
    }

    /// <summary>
    /// 订单内容
    /// </summary>
    public struct OrderData
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string playerName { get; set; }

        /// <summary>
        /// 所属的游戏分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 游戏代码
        /// </summary>
        public string gameCode { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public GameCurrency? currency { get; set; }

        /// <summary>
        /// 订单的下单时间
        /// </summary>
        public long createTime { get; set; }

        /// <summary>
        /// 订单的结算时间
        /// </summary>
        public long settleTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GameAPIOrderStatus status { get; set; }

        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal betMoney { get; set; }

        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal betAmount { get; set; }

        /// <summary>
        /// 盈亏
        /// </summary>
        public decimal money { get; set; }

        /// <summary>
        /// 玩家的IP
        /// </summary>
        public string clientIp { get; set; }


        /// <summary>
        /// Hash唯一字符串，判断订单数据是否需要更新
        /// </summary>
        public string hash { get; set; }

        /// <summary>
        /// 原始数据
        /// </summary>
        public string rawData { get; set; }

        /// <summary>
        /// 扩展的关键词数据
        /// </summary>
        public IOrderData data { get; set; }
    }
}
