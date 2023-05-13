using BetWin.Game.API.Enums;
using BetWin.Game.API.Exceptions;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.API.Handlers
{
    /// <summary>
    /// EVO 真人
    /// </summary>
    public class EVOLive : GameBase
    {
        public EVOLive(string jsonString) : base(jsonString)
        {
        }

        #region ========  参数配置  ========

        [Description("网关")]
        public string APIHost { get; set; } = "http://staging.evolution.asia-live.com";

        [Description("游戏记录")]
        public string GameAPI { get; set; } = "https://stage-admin.asia-live.com";

        [Description("商户ID")]
        public string? MerchantID { get; set; }

        /// <summary>
        /// Casino key for User Authentication service, provided by EVO
        /// </summary>
        [Description("CasinoKey")]
        public string? CasinoKey { get; set; }

        /// <summary>
        /// API token for User Authentication service, provided by EVO
        /// </summary>
        [Description("APIToken")]
        public string? APIToken { get; set; }

        #endregion

        public override Dictionary<Language, string> Languages => new Dictionary<Language, string>()
        {
        };

        public override Dictionary<Currency, string> Currencies => new Dictionary<Currency, string>()
        {

        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            throw new NotImplementedException();
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            throw new NotImplementedException();
        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            throw new NotImplementedException();
        }

        public override LoginResponse Login(LoginModel request)
        {
            string body = JsonConvert.SerializeObject(new
            {
                uuid = Guid.NewGuid().ToString("N"),
                player = new
                {
                    id = request.PlayerName,
                    update = true,
                    firstName = request.PlayerName,
                    lastName = request.PlayerName,
                    country = "CN",
                    language = this.Languages.Get(request.Language, "en"),
                    currency = this.Currencies.Get(request.Currency, "VND"),
                    session = new
                    {
                        id = request.PlayerName,
                        ip = request.clientIp ?? "0.0.0.0"
                    }
                },
                config = new
                {
                    game = new
                    {
                        table = new
                        {
                            id = request.StartCode
                        }
                    }
                }
            });

            GameResponse response = this.Request(new GameRequest
            {
                Url = $"/ua/v1/{this.CasinoKey}/{this.APIToken}",
                Data = body,
                Method = APIMethod.Login,
                Option = new Dictionary<string, string>()
                    {
                        {"Content-Type","application/json" }
                    }
            });

            if (response != GameResultCode.Success) return new LoginResponse(response)
            {
                 Message = response.Message
            };
            login? login = JsonConvert.DeserializeObject<login>(response);
            if (login == null) return new LoginResponse(GameResultCode.Error)
            {
                 Message = response.Message
            };

            return new LoginResponse(GameResultCode.Success)
            {
                Url = login.entry,
                Method = LoginMethod.Redirect
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            throw new NotImplementedException();
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            throw new NotImplementedException();
        }

        #region ========  工具方法  ========

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            message = string.Empty;
            JObject json = JObject.Parse(result);
            if (!json.ContainsKey("errors")) return GameResultCode.Success;

            JArray? errors = json["errors"]?.Value<JArray>();
            if (errors == null || !errors.Any()) return GameResultCode.Error;

            message = errors[0]?["message"]?.Value<string>() ?? string.Empty;
            string code = errors[0]?["code"]?.Value<string>() ?? string.Empty;

            return code switch
            {
                "G.9" => GameResultCode.IPInvalid,
                _ => GameResultCode.Error
            };
        }


        protected override string RequestAPI(GameRequest request)
        {
            string result = string.Empty;
            HttpMethod? method = null;
            switch (request.Method)
            {
                case APIMethod.Login:
                    method = HttpMethod.Post;
                    break;
                case APIMethod.Logout:
                    break;
            }

            if (method == null) throw new GameException($"未实现方法 {request.Method}");

            string url = $"{this.APIHost}{request.Url}";
            switch (method.Method)
            {
                case "POST":
                    result = NetAgent.PostAsync(url, request.Data, Encoding.UTF8, request.Option).Result;
                    break;
                case "GET":
                    result = NetAgent.GetAsync(url, Encoding.UTF8, request.Option).Result;
                    break;
            }

            return result;
        }


        #endregion

        #region ========  实体类  ========

        /// <summary>
        /// 登录返回
        /// </summary>
        class login
        {
            public string entry { get; set; }
        }

        //"{\"errors\":[{\"code\":\"G.9\",\"message\":\"Clients IP address have been rejected\"}]}"
        class error
        {
            public errors[] errors { get; set; }
        }

        class errors
        {
            public string code { get; set; }

            public string message { get; set; }
        }

        #endregion
    }
}
