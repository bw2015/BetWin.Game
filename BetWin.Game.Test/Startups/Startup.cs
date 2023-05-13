using BetWin.Game.API;
using BetWin.Game.API.Enums;
using BetWin.Game.API.Requests;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Http;
using SP.StudioCore.Model;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Tools;
using SP.StudioCore.Web;

namespace BetWin.Game.Test.Startups
{
    public class Startup : StartBase
    {
        public Startup(HttpContext context) : base(context)
        {
        }

        /// <summary>
        /// 测试登录方法
        /// </summary>
        [HttpPost]
        public Result Login([FromQuery] GameType type, [FromForm] string username, [FromForm] string setting)
        {
            var game = GameFactory.GetGame(type, setting);
            var loginResponse = game?.Login(new LoginModel(0)
            {
                clientIp = IPAgent.CreateRandomChinaIPv4(),
                Currency = Currency.VND,
                Language = Language.CHN,
                Platform = ((PlatformType)((byte)this.context.GetPlatform())),
                PlayerName = username,
                StartCode = ""
            });
            if (loginResponse == null) throw new ResultException($"找不到游戏实现对象 - {type}");
            return this.GetResultContent(loginResponse);
        }
    }
}
