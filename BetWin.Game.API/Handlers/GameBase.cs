using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BetWin.Game.API.Handlers
{
    public interface IGameHandler
    {
        public Dictionary<Language, string> Languages { get; }

        public Dictionary<Currency, string> Currencies { get; }

        /// <summary>
        /// 注册
        /// </summary>
        public RegisterResponse Register(RegisterModel request);

        /// <summary>
        /// 请求进入游戏
        /// </summary>
        public LoginResponse Login(LoginModel request);

        /// <summary>
        /// 从游戏中退出（如果接口不支持则返回成功）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LogoutResponse Logout(LogoutModel request);

        /// <summary>
        /// 余额查询
        /// </summary>
        public BalanceResponse Balance(BalanceModel request);

        /// <summary>
        /// 转账
        /// </summary>
        public TransferResponse Transfer(TransferModel request);

        /// <summary>
        /// 检查转账是否成功
        /// </summary>
        public CheckTransferResponse CheckTransfer(CheckTransferModel request);

        /// <summary>
        /// 采集订单（对外执行方法）
        /// </summary>
        public OrderResult GetOrder(QueryOrderModel request);


        /// <summary>
        /// 回调支持
        /// </summary>
        public string Callback(HttpContextModel context);
    }

    public abstract class GameBase : IGameHandler
    {
        public GameBase(string jsonString)
        {
            JsonConvert.PopulateObject(jsonString, this);
        }

        /// <summary>
        /// 当前线路所支持的语种
        /// </summary>
        public abstract Dictionary<Language, string> Languages { get; }

        /// <summary>
        /// 线路所支持的币种
        /// </summary>
        public abstract Dictionary<Currency, string> Currencies { get; }

        /// <summary>
        /// 转换币种
        /// </summary>
        protected virtual decimal ConvertCurrency(decimal money, ref Currency currency)
        {
            return money;
        }

        protected virtual Currency ConvertCurrency(string currency)
        {
            return this.Currencies?.FirstOrDefault(t => t.Value == currency).Key ?? (Currency)0;
        }

        #region ========  通用的方法  ========

        /// <summary>
        /// 注册
        /// </summary>
        public abstract RegisterResponse Register(RegisterModel request);

        /// <summary>
        /// 请求进入游戏
        /// </summary>
        public abstract LoginResponse Login(LoginModel request);

        /// <summary>
        /// 从游戏中退出（如果接口不支持则返回成功）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract LogoutResponse Logout(LogoutModel request);

        /// <summary>
        /// 余额查询
        /// </summary>
        public abstract BalanceResponse Balance(BalanceModel request);

        /// <summary>
        /// 转账
        /// </summary>
        public abstract TransferResponse Transfer(TransferModel request);

        /// <summary>
        /// 检查转账是否成功
        /// </summary>
        public abstract CheckTransferResponse CheckTransfer(CheckTransferModel request);

        /// <summary>
        /// 采集订单（对外执行方法）
        /// </summary>
        public abstract OrderResult GetOrder(QueryOrderModel request);


        /// <summary>
        /// 回调支持
        /// </summary>
        public virtual string Callback(HttpContextModel context)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// 发起http请求
        /// </summary>
        /// <param name="method">动作类型</param>
        /// <param name="postData"></param>
        /// <param name="option"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected virtual GameResponse Request(GameRequest request)
        {
            Stopwatch sw = Stopwatch.StartNew();
            GameResponse response = default;
            HttpResult result = default;
            try
            {
                result = this.RequestAPI(request);
                if (!result) throw new Exception(result);
                return response = new GameResponse(result, this.GetResultCode(result, out string message), message, (int)sw.ElapsedMilliseconds, request);
            }
            catch (Exception ex)
            {
                return response = new GameResponse(result, GameResultCode.Exception, ex.Message, (int)sw.ElapsedMilliseconds, request);
            }
            finally
            {
                // 写入API请求日志
                Console.WriteLine($"======== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ========");
                Console.WriteLine(request.Url);
                Console.WriteLine(response.ToJson());
            }
        }

        /// <summary>
        /// 获取返回内容的代码
        /// </summary>
        protected abstract GameResultCode GetResultCode(string result, out string message);

        protected abstract HttpResult RequestAPI(GameRequest request);
    }
}
