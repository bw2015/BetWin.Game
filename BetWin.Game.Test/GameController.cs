using BetWin.Game.API;
using BetWin.Game.API.Enums;
using BetWin.Game.API.Handlers;
using BetWin.Game.API.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SP.StudioCore.Data;
using SP.StudioCore.Enums;
using SP.StudioCore.Mvc;
using SP.StudioCore.Types;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Linq;

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
        /// 获取所有的游戏类型
        /// </summary>
        /// <returns></returns>
        public IActionResult GetGames()
        {
            Dictionary<GameType, string> games = new Dictionary<GameType, string>();
            foreach (GameType game in Enum.GetValues<GameType>())
            {
                games.Add(game, game.GetDescription());
            }
            return new JsonResult(games);
        }

        /// <summary>
        /// 获取配置参数
        /// </summary>
        public IActionResult getSetting([FromForm] GameType game)
        {
            var handler = this.GetHandler(game, "{}");
            var list = new List<object>();
            if (handler == null) return new JsonResult(list);

            foreach (PropertyInfo property in handler.GetType().GetProperties())
            {
                if (!property.CanWrite) continue;
                string name = property.Name;
                string description = property.GetAttribute<DescriptionAttribute>()?.Description ?? name;
                string value = property.GetValue(handler)?.ToString() ?? string.Empty;

                list.Add(new
                {
                    name,
                    description,
                    value
                });
            }

            return new JsonResult(list);
        }

        /// <summary>
        /// 获取参数配置
        /// </summary>
        public IActionResult getModel([FromForm] string model)
        {
            Assembly assembly = typeof(GameFactory).Assembly;
            Type? type = assembly.GetType($"BetWin.Game.API.Requests.{model}");
            if (type == null) return new NotFoundResult();
            var list = new List<string>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (!property.CanWrite) continue;
                list.Add(property.Name);
            }
            return new JsonResult(list);
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

        [HttpPost]
        public IActionResult Transfer([FromQuery] GameType game, [FromForm] string setting, [FromJson] TransferModel model)
        {
            IGameHandler? handler = this.GetHandler(game, setting);
            var response = handler.Transfer(model);
            return new JsonResult(response);
        }

        [HttpPost]
        public IActionResult CheckTransfer([FromQuery] GameType game, [FromForm] string setting, [FromJson] CheckTransferModel model)
        {
            IGameHandler? handler = this.GetHandler(game, setting);
            var response = handler.CheckTransfer(model);
            return new JsonResult(response);
        }
    }
}
