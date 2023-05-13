using BetWin.Game.API.Enums;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// 玩家信息对象
    /// </summary>
    public class PlayerInfoModel
    {
        /// <summary>
        /// 游戏用户名
        /// </summary>
        public string playerName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency currency { get; set; }
    }
}
