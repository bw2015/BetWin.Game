using BetWin.Game.API;
using BetWin.Game.API.Enums;
using BetWin.Game.API.Handlers;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Http;
using SP.StudioCore.Model;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Tools;
using SP.StudioCore.Web;
using StackExchange.Redis;
using System.Security.Cryptography.Xml;

namespace BetWin.Game.Test.Startups
{
    public class Startup : StartBase
    {
        public Startup(HttpContext context) : base(context)
        {
        }

        private IGameHandler GetGetHandler(GameType type, string setting)
        {
            IGameHandler? game = GameFactory.GetGame(type, setting);
            if (game == null) throw new ResultException($"找不到游戏实现对象 - {type}");
            return game;
        }

        /// <summary>
        /// 测试登录方法
        /// </summary>
        [HttpPost]
        public Result Login([FromQuery] GameType type, [FromForm] string username, [FromForm] string setting)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            LoginResponse? loginResponse = game?.Login(new LoginModel(0)
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

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="type"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public Result Balance([FromQuery] GameType type, [FromForm] string username, [FromForm] string setting)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            BalanceResponse? balanceResponse = game?.Balance(new BalanceModel(0) { PlayerName = username });
            if (balanceResponse == null) throw new ResultException($"找不到游戏实现对象 - {type}");
            return this.GetResultContent(balanceResponse);
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="type"></param>
        /// <param name="username"></param>
        /// <param name="money">正数转入/负数转出</param>
        /// <returns></returns>
        [HttpPost]
        public Result Transfer([FromQuery] GameType type, [FromForm] string setting, [FromForm] string username, [FromForm] decimal money, [FromForm] Currency? currency)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            TransferResponse? transfer = game?.Transfer(new TransferModel(0)
            {
                Currency = currency ?? Currency.CNY,
                Money = money,
                OrderID = WebAgent.GetTimestamps().ToString(),
                PlayerName = username
            });
            return this.GetResultContent(transfer);
        }

        /// <summary>
        /// 检查转账状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="setting"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public Result CheckTransfer([FromQuery] GameType type, [FromForm] string setting, [FromForm] string orderId)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            CheckTransferResponse? checkTransfer = game?.CheckTransfer(new CheckTransferModel(0)
            {
                OrderID = orderId
            });
            return this.GetResultContent(checkTransfer);
        }

        /// <summary>
        /// 采集订单
        /// </summary>
        /// <param name="type"></param>
        /// <param name="setting"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        [HttpPost]
        public Result GetOrder([FromQuery] GameType type, [FromForm] string setting, [FromForm] DateTime startDate)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            var orders = game?.GetOrder(new QueryOrderModel
            {
                StartTime = WebAgent.GetTimestamps(startDate),
                EndTime = WebAgent.GetTimestamps(startDate.AddDays(1))
            });
            return this.GetResultContent(orders);
        }
    }
}
