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
            {Language.CHN,"zh" },
            {Language.ENG,"en" },
            {Language.VI,"vi" },
            {Language.TH,"th" },
            {Language.IND,"id" },
            {Language.HI,"hi" },
        };

        public override Dictionary<Currency, string> Currencies => new Dictionary<Currency, string>()
        {
            { Currency.CNY,"CNY" },
            { Currency.VND,"VND" },
            { Currency.USD,"USD" },
            { Currency.THB,"THB" },
            { Currency.IDR,"IDR" },
            { Currency.INR,"INR" },
        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            GameResponse response = this.Request(new GameRequest
            {
                Method = APIMethod.Balance,
                Url = $"/api/ecashier?cCode=RWA&ecID={this.CasinoKey}&euID={request.PlayerName}&output=0"
            });

            if (response != GameResultCode.Success) return new BalanceResponse(response);
            JObject info = JObject.Parse(response);
            if (!info.ContainsKey("userbalance")) return new BalanceResponse(GameResultCode.Error);
            return new BalanceResponse(response)
            {
                Balance = info["userbalance"]?["tbalance"]?.Value<decimal>() ?? decimal.Zero
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            GameResponse response = this.Request(new GameRequest
            {
                Method = APIMethod.CheckTransfer,
                Url = $"/api/ecashier?cCode=TRI&ecID={this.CasinoKey}&euID={request.PlayerName}&output=0&eTransID={request.OrderID}"
            });

            JObject? json = JObject.Parse(response);
            return new CheckTransferResponse(response)
            {
                TransferID = request.OrderID,
                Money = json?["transaction"]?["amount"]?.Value<decimal>(),
                Status = response.Code switch
                {
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    _ => GameAPITransferStatus.Faild
                }
            };
        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            DateTime startDate = WebAgent.GetTimestamps(request.StartTime);
            DateTime endDate = WebAgent.GetTimestamps(request.EndTime);

            if (endDate > DateTime.Now.AddMinutes(5)) endDate = DateTime.Now.AddMinutes(-5);
            if (endDate - startDate > TimeSpan.FromDays(1)) endDate = startDate.AddDays(1);

            startDate = startDate.GetTimeZone(0);
            endDate = endDate.GetTimeZone(0);

            GameResponse response = this.Request(new GameRequest
            {
                Method = APIMethod.GetOrder,
                Url = $"{this.GameAPI}/api/gamehistory/v1/casino/games?startDate={startDate:yyyy-MM-ddTHH:mm:ssZ}&endDate={endDate:yyyy-MM-ddTHH:mm:ssZ}",
                Option = new Dictionary<string, string>()
                {
                    { "Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this.CasinoKey}:{this.APIToken}"))}" }
                }
            });

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
            return new RegisterResponse(GameResultCode.Success)
            {
                PlayerName = request.UserName
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            string cCode = "ECR";
            if (request.Money < 0)
            {
                cCode = "EDB";
            }
            string url = $"/api/ecashier?cCode={cCode}&ecID={this.CasinoKey}&euID={request.PlayerName}&amount={Math.Abs(request.Money)}&eTransID={request.OrderID}&output=0";

            GameResponse response = this.Request(new GameRequest
            {
                Url = url,
                Method = APIMethod.Transfer
            });

            JObject? json = response ? JObject.Parse(response) : null;

            return new TransferResponse(response.Code)
            {
                Money = request.Money,
                OrderID = request.OrderID,
                Balance = json?["transfer"]?["balance"]?.Value<decimal>(),
                TransferID = json?["transfer"]?["etransid"]?.Value<string>() ?? string.Empty,
                PlayerName = request.PlayerName,
                Status = response.Code switch
                {
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    _ => GameAPITransferStatus.Faild
                }
            };
        }

        #region ========  工具方法  ========

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            message = string.Empty;
            JObject json = JObject.Parse(result);

            string code = string.Empty;
            if (json.ContainsKey("error"))
            {
                code = message = json["error"]?["errormsg"]?.Value<string>() ?? string.Empty;
                if (message.StartsWith("Transaction is not found for id"))
                {
                    code = "Transaction is not found";
                }
            }
            else if (json.ContainsKey("errors"))
            {
                JArray? errors = json["errors"]?.Value<JArray>();
                if (errors == null || !errors.Any()) return GameResultCode.Error;
                message = errors[0]?["message"]?.Value<string>() ?? string.Empty;
                code = errors[0]?["code"]?.Value<string>() ?? string.Empty;
            }
            else if (json.ContainsKey("transfer"))
            {
                code = json["transfer"]?["result"]?.Value<string>() ?? string.Empty;
                if (code == "N")
                {
                    code = message = json["transfer"]?["errormsg"]?.Value<string>() ?? string.Empty;
                }
                if (code.StartsWith("Insufficient funds:")) code = "Insufficient funds";
            }
            else if (json.ContainsKey("_exception"))
            {
                code = "_exception";
                message = json["_exception"]?.Value<string>() ?? string.Empty;
            }

            return code switch
            {
                "G.9" => GameResultCode.IPInvalid,
                "Invalid casino" => GameResultCode.NoMerchant,
                "Invalid user" => GameResultCode.NoPlayer,
                "Amount not specified" => GameResultCode.MoneyInvalid,
                "Insufficient funds" => GameResultCode.NoBalance,
                "Transaction is not found" => GameResultCode.OrderNotFound,
                "_exception" => GameResultCode.Exception,
                "Y" => GameResultCode.Success,
                "" => GameResultCode.Success,
                _ => GameResultCode.Error
            };
        }


        protected override HttpResult RequestAPI(GameRequest request)
        {
            HttpResult result = default;
            HttpMethod? method;
            switch (request.Method)
            {
                case APIMethod.Login:
                    method = HttpMethod.Post;
                    break;
                default:
                    method = HttpMethod.Get;
                    break;
            }

            if (method == null) throw new GameException($"未实现方法 {request.Method}");

            string url = request.Url;
            if (!url.StartsWith("http")) url = $"{this.APIHost}{request.Url}";
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
