using BetWin.Game.API.Enums;

namespace BetWin.Game.API.Requests
{
    /// <summary>
    /// 登录请求参数
    /// </summary>
    public class LoginModel : GameModelBase
    {
        public LoginModel(int siteId) : base(siteId)
        {
        }

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 玩家的密码（如果有的话）
        /// </summary>
        public string PlayerPassword { get; set; }

        /// <summary>
        /// 平台类型
        /// </summary>
        public PlatformType Platform { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 语种
        /// </summary>
        public LanguageType Language { get; set; }

        /// <summary>
        /// 要进入的游戏代码
        /// </summary>
        public string StartCode { get; set; }

        /// <summary>
        /// 登录用户的IP
        /// </summary>
        public string clientIp { get; set; } = "0.0.0.0";
    }
}
