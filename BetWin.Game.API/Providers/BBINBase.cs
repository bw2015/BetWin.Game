using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using SP.StudioCore.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace BetWin.Game.API.Providers
{
    /// <summary>
    /// BBIN基类
    /// </summary>
    public abstract class BBINBase : GameBase
    {
        #region ========  网关参数  ========

        [Description("网关")]
        public string gateway { get; set; }

        /// <summary>
        /// 网站名称
        /// </summary>
        [Description("WebSite")]
        public string WebSite { get; set; }

        /// <summary>
        /// 上层帐号
        /// </summary>
        [Description("UpperName")]
        public string UpperName { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        [Description("Prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// 创建会员 KeyA=2 KeyC=6
        /// </summary>
        [Description("CreateMember")]
        public string CreateMemberKey { get; set; }

        /// <summary>
        /// 登录
        /// </summary>
        [Description("Login")]
        public string LoginKey { get; set; }

        /// <summary>
        /// 产生会员SessionID  KeyA=7 KeyC=2
        /// </summary>
        [Description("CreateSession")]
        public string CreateSessionKey { get; set; }
        /// <summary>
        /// 登录2
        /// </summary>
        [Description("Login2")]
        public string Login2Key { get; set; }


        /// <summary>
        /// 转账 KeyA=3 KeyC=7
        /// </summary>
        [Description("Transfer")]
        public string TransferKey { get; set; }

        /// <summary>
        /// 获取余额 KeyA=1 KeyC=8
        /// </summary>
        [Description("CheckUsrBalance")]
        public string CheckUsrBalanceKey { get; set; }

        /// <summary>
        /// 检查转账 KeyA=2 KeyC=3
        /// </summary>
        [Description("CheckTransfer")]
        public string CheckTransferKey { get; set; }


        /// <summary>
        /// BB游戏连接 KeyA=6 KeyC=7
        /// </summary>
        [Description("GameUrlKey")]
        public string GameUrlKey { get; set; }

        /// <summary>
        /// 采集记录 KeyA=4 KeyC=8
        /// </summary>
        [Description("WagersRecord")]
        public string WagersRecordKey { get; set; }

        #endregion

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>()
        {
            {GameLanguage.CHN,"zh-cn" },
            {GameLanguage.THN,"zh-tw" },
            {GameLanguage.ENG,"en-us" },
            {GameLanguage.JA,"euc-jp" },
            {GameLanguage.KO,"ko" },
            {GameLanguage.TH,"th" },
            {GameLanguage.VI,"vi" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>()
        {
            {GameCurrency.CNY,"CNY" }
        };

        protected BBINBase(string jsonString) : base(jsonString)
        {
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            string playerName = this.CreateUserName(request.Prefix, request.UserName, 20);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"uppername",this.UpperName },
                {"username",playerName},
                {"ingress",1 },
                {"key",getKey(2,playerName,CreateMemberKey,6) }
            };
            var register = this.get<register>(APIMethod.Register, "CreateMember", data, out GameResultCode code, out string message);
            return new RegisterResponse(code)
            {
                Message = message,
                PlayerName = playerName
            };
        }

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"username",request.PlayerName },
                {"uppername",this.UpperName },
                {"page",string.Empty },
                {"pagelimit",string.Empty },
                {"key",this.getKey(1,request.PlayerName,CheckUsrBalanceKey, 8) }
            };

            response<balance[]>? balance = this.get<balance[]>(APIMethod.Balance, "CheckUsrBalance", data, out GameResultCode code, out string message);

            return new BalanceResponse(code)
            {
                Currency = request.Currency,
                Balance = balance?.data?.FirstOrDefault()?.Balance ?? 0,
                Message = message
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"username",request.PlayerName },
                {"uppername",this.UpperName },
                {"remitno",request.OrderID },
                {"action", request.Money>0 ? "IN" : "OUT"},
                {"remit",Math.Abs( request.Money) },
                {"key", getKey(3,request.PlayerName+request.OrderID,TransferKey,7)}
            };

            response<transfer>? transfer = this.get<transfer>(APIMethod.Transfer, "Transfer", data, out GameResultCode code, out string message);

            return new TransferResponse(code)
            {
                Message = message,
                Money = request.Money,
                OrderID = request.OrderID,
                Currency = request.Currency,
                PlayerName = request.PlayerName,
                Status = code switch
                {
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    _ => GameAPITransferStatus.Faild
                },
                TransferID = request.OrderID
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"transid",request.OrderID },
                {"key",this.getKey(2,string.Empty,CheckTransferKey,3) }
            };
            response<checkTransfer>? response = this.get<checkTransfer>(APIMethod.CheckTransfer, "CheckTransfer", data, out GameResultCode code, out string message);

            return new CheckTransferResponse(code)
            {
                Status = response?.data?.Status switch
                {
                    null => GameAPITransferStatus.Unknow,
                    1 => GameAPITransferStatus.Success,
                    _ => GameAPITransferStatus.Faild
                },
                TransferID = response?.data?.TransID ?? request.OrderID
            };
        }

        #region ========  工具方法  ========

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            response? response = result.ToJson<response>();
            if (response == null)
            {
                message = result;
                return GameResultCode.Exception;
            }
            if (response.result)
            {
                message = string.Empty;
                return GameResultCode.Success;
            }

            response<responseData>? res = result.ToJson<response<responseData>>();

            message = res?.data?.Message ?? string.Empty;
            return res?.data?.Code switch
            {
                10002 => GameResultCode.NoBalance,
                10003 => GameResultCode.TransferFalid,
                11000 => GameResultCode.Repeated,
                11100 => GameResultCode.Success,
                21001 => GameResultCode.DuplicatePlayerName,
                21100 => GameResultCode.Success,
                22002 => GameResultCode.NoPlayer,
                22006 => GameResultCode.NoMerchant,
                22011 => GameResultCode.NoPlayer,
                22013 => GameResultCode.NoPlayer,
                40005 => GameResultCode.NoMerchant,
                44000 => GameResultCode.SignInvalid,
                44444 => GameResultCode.Maintenance,
                44900 => GameResultCode.IPInvalid,
                40001 => GameResultCode.GameClose,
                45007 => GameResultCode.GameClose,
                47005 => GameResultCode.ParameterInvalid,
                47003 => GameResultCode.ParameterInvalid,
                44001 => GameResultCode.ParameterInvalid,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                return client.Get($"{request.Url}?{request.Data}", new Dictionary<string, string>());
            }
        }

        protected response<T>? get<T>(APIMethod method, string action, Dictionary<string, object> data, out GameResultCode code, out string message) where T : class
        {
            if (!data.ContainsKey("website")) data.Add("website", this.WebSite);
            if (!data.ContainsKey("uppername")) data.Add("uppername", this.UpperName);

            GameResponse result = this.Request(new GameRequest()
            {
                Url = $"{this.gateway}/app/WebService/json/display.php/{action}",
                Data = data.ToQueryString(),
                Method = method
            });

            code = this.GetResultCode(result.Content, out message);
            return result.Content.ToJson<response<T>>();
        }

        /// <summary>
        /// 获取登录的session
        /// </summary>
        protected string? getSessionId(LoginModel request, out GameResultCode code, out string message)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"website",this.WebSite },
                {"uppername",this.UpperName },
                {"username",request.PlayerName },
                {"lang",Languages.Get(request.Language,"zh-cn") },
                {"ingress",this.getIngress(request.Platform) },
                {"key",this.getKey(7,request.PlayerName,CreateSessionKey,2) }
            };
            response<session>? session = this.get<session>(APIMethod.Login, "CreateSession", data, out code, out message);
            return session?.data?.sessionid;
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        protected string getKey(int aLength, string value, string key, int cLength)
        {
            string timesTamp = DateTime.Now.AddHours(-12).ToString("yyyyMMdd");
            string keyA = Guid.NewGuid().ToString("N")[..aLength];
            string keyC = Guid.NewGuid().ToString("N")[..cLength];
            string valueB = WebSite + value + key + timesTamp;
            string keyB = valueB.toMD5().ToLower();
            return string.Concat(keyA, keyB, keyC);
        }

        /// <summary>
        /// 获取设备号
        /// </summary>
        /// <param name="type"></param>
        /// <returns>1、PC 2、Mobile</returns>
        protected int getIngress(GamePlatform type)
        {
            return type switch
            {
                GamePlatform.PC => 1,
                GamePlatform.Mobile => 2,
                _ => 9
            };
        }

        /// <summary>
        /// 转换订单状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected GameAPIOrderStatus ConvertOrderStatus(string status)
        {
            return status switch
            {
                "C" => GameAPIOrderStatus.Return,
                "D" => GameAPIOrderStatus.Return,
                "2" => GameAPIOrderStatus.Return,
                "5" => GameAPIOrderStatus.Return,
                "-1" => GameAPIOrderStatus.Return,
                "W" => GameAPIOrderStatus.Win,
                "4" => GameAPIOrderStatus.Win,
                "L" => GameAPIOrderStatus.Lose,
                "3" => GameAPIOrderStatus.Lose,
                _ => GameAPIOrderStatus.Wait
            };
        }

        #endregion

        #region ========  实体类  ========

        /// <summary>
        /// 错误返回
        /// </summary>
        protected class response
        {
            public bool result { get; set; }

            /// <summary>
            /// 分页参数
            /// </summary>
            public pagination? pagination { get; set; }
        }

        /// <summary>
        /// 分页参数
        /// \"pagination\":{\"Page\":0,\"PageLimit\":10000,\"TotalNumber\":\"0\",\"TotalPage\":0}
        /// </summary>
        protected class pagination
        {
            public int Page { get; set; }

            public int PageLimit { get; set; }

            public int TotalNumber { get; set; }

            public int TotalPage { get; set; }
        }

        /// <summary>
        /// {"result":false,"data":{"Code":"44001","Message":"The parameters are not complete.."}}
        /// </summary>
        protected class responseData
        {
            public int Code { get; set; }

            public string? Message { get; set; }
        }

        protected class response<T> : response where T : class
        {
            public bool result { get; set; }

            public T? data { get; set; }
        }

        /// <summary>
        /// 登录
        /// </summary>
        protected class session
        {
            public string? sessionid { get; set; }
        }

        protected class login
        {
            public string pc { get; set; }

            public string mobile { get; set; }

            public string rwd { get; set; }
        }

        protected class register : responseData
        {

        }

        protected class balance
        {
            public decimal Balance { get; set; }
        }

        protected class transfer : responseData
        {

        }

        protected class checkTransfer
        {
            public int Status { get; set; }

            public string TransID { get; set; }
        }

        #endregion
    }
}
