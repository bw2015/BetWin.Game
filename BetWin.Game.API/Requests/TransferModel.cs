using BetWin.Game.API.Enums;

namespace BetWin.Game.API.Requests
{
    /// <summary>
    /// 转账实体
    /// </summary>
    public class TransferModel : GameModelBase
    {
        public TransferModel(int siteId) : base(siteId)
        {
        }

        /// <summary>
        /// 币种
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 转账金额（转入为正数/转出为负数）
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string PlayerName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// 本地的订单号
        /// </summary>
        public string OrderID { get; set; }
    }
}
