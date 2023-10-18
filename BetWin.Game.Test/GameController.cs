using BetWin.Game.API;
using BetWin.Game.API.Enums;
using BetWin.Game.API.Handlers;
using BetWin.Game.API.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SP.StudioCore.Mvc;

namespace BetWin.Game.Test
{
    [Route("[controller]/[action]"), ApiController]
    public class GameController : ControllerBase
    {
        private IGameHandler GetHandler(GameType game, string setting)
        {
            IGameHandler? handler = GameFactory.GetGame(game, setting);
            if (handler == null)
            {
                throw new Exception($"找不到游戏类型 => {game}");
            }
            return handler;
        }


        /// <summary>
        /// 登录请求
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromQuery] GameType game, [FromForm] string setting, [FromJson] LoginModel model)
        {
            IGameHandler? handler = this.GetHandler(game, setting);
            var response = handler.Login(model);
            return new JsonResult(response);
        }

        [HttpPost]
        public IActionResult Logout([FromQuery] GameType game, [FromForm] string setting, [FromJson] LogoutModel model)
        {
            IGameHandler? handler = this.GetHandler(game, setting);
            var response = handler.Logout(model);
            return new JsonResult(response);
        }

        [HttpPost]
        public IActionResult Register([FromQuery] GameType game, [FromForm] string setting, [FromJson] RegisterModel model)
        {
            IGameHandler? handler = this.GetHandler(game, setting);
            var response = handler.Register(model);
            return new JsonResult(response);
        }

        [HttpPost]
        public IActionResult Balance([FromQuery] GameType game, [FromForm] string setting, [FromJson] BalanceModel model)
        {
            IGameHandler? handler = this.GetHandler(game, setting);
            var response = handler.Balance(model);
            return new JsonResult(response);
        }
    }
}
