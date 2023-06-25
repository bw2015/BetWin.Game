using BetWin.Game.API;
using BetWin.Game.API.Enums;
using BetWin.Game.API.Handlers;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Enums;
using SP.StudioCore.Http;
using SP.StudioCore.Mapper;
using SP.StudioCore.Model;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Tools;
using SP.StudioCore.Types;
using SP.StudioCore.Web;
using StackExchange.Redis;
using System.ComponentModel;
using System.Security.Cryptography.Xml;
using Currency = BetWin.Game.API.Enums.Currency;
using Language = BetWin.Game.API.Enums.Language;
using PlatformType = BetWin.Game.API.Enums.PlatformType;

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
        /// 获取所有的游戏枚举
        /// </summary>
        /// <returns></returns>
        public Result GetGameType()
        {
            Dictionary<GameType, string> data = new();
            foreach (GameType game in Enum.GetValues<GameType>())
            {
                data.Add(game, game.GetDescription());
            }
            return this.GetResultContent(data);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public Result GetGameSetting([FromForm] GameType type)
        {
            IGameHandler? game = this.GetGetHandler(type, string.Empty);
            Dictionary<string, string> setting = game.GetProperties().Where(t => t.HasAttribute<DescriptionAttribute>()).ToDictionary(t => t.Name, t => t.GetAttribute<DescriptionAttribute>()?.Description ?? t.Name);
            return this.GetResultContent(setting);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public Result Register([FromQuery] GameType type, [FromForm] string setting, [FromForm] string prefix, [FromForm] string username, [FromForm] Currency? currency, [FromForm] Language? language)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            RegisterResponse registerResponse = game.Register(new RegisterModel(0)
            {
                UserName = username,
                Prefix = prefix,
                Currency = currency ?? Currency.CNY
            });
            return this.GetResultContent(registerResponse);
        }

        /// <summary>
        /// 测试登录方法
        /// </summary>
        [HttpPost]
        public Result Login([FromQuery] GameType type, [FromForm] string username, [FromForm] Currency? currency, [FromForm] Language? language, [FromForm] string setting)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            LoginResponse loginResponse = game.Login(new LoginModel(0)
            {
                clientIp = IPAgent.CreateRandomChinaIPv4(),
                Currency = currency ?? Currency.CNY,
                Language = language ?? Language.CHN,
                Platform = ((PlatformType)((byte)this.context.GetPlatform())),
                PlayerName = username,
                StartCode = ""
            });
            return this.GetResultContent(loginResponse);
        }

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="type"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public Result Balance([FromQuery] GameType type, [FromForm] string username, [FromForm] Currency? currency, [FromForm] string setting)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            BalanceResponse? balanceResponse = game?.Balance(new BalanceModel(0)
            {
                PlayerName = username,
                Currency = currency ?? Currency.CNY
            });
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
        public Result Transfer([FromQuery] GameType type, [FromForm] string setting, [FromForm] string username, [FromForm] decimal money, [FromForm] Currency? currency, [FromForm] string? orderId)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            TransferResponse? transfer = game?.Transfer(new TransferModel(0)
            {
                Currency = currency ?? Currency.CNY,
                Money = money,
                OrderID = orderId ?? WebAgent.GetTimestamps().ToString(),
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
        public Result GetOrder([FromQuery] GameType type, [FromForm] string setting, [FromForm] DateTime startTime, [FromForm] DateTime? endTime = null)
        {
            IGameHandler game = this.GetGetHandler(type, setting);
            endTime ??= startTime.AddHours(1);
            OrderResult? orders = game?.GetOrder(new QueryOrderModel
            {
                StartTime = WebAgent.GetTimestamps(startTime),
                EndTime = WebAgent.GetTimestamps(endTime.Value)
            });
            if (orders == null) throw new ResultException("查询发生错误");
            return this.GetResultContent(orders);
        }
    }
}
