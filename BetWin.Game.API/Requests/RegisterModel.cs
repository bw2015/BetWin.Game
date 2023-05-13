using BetWin.Game.API.Enums;

namespace BetWin.Game.API.Requests
{
    public class RegisterModel : GameModelBase
    {
        public RegisterModel(int siteId) : base(siteId)
        {
        }

        /// <summary>
        /// 商户前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户的币种
        /// </summary>
        public Currency Currency { get; set; }
    }
}
