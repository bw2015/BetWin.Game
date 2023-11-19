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

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>()
        {
            {GameLanguage.CHN,"zh" },
            {GameLanguage.ENG,"en" },
            {GameLanguage.VI,"vi" },
            {GameLanguage.TH,"th" },
            {GameLanguage.IND,"id" },
            {GameLanguage.HI,"hi" },
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>()
        {
            { GameCurrency.CNY,"CNY" },
            { GameCurrency.VND,"VND" },
            { GameCurrency.USD,"USD" },
            { GameCurrency.THB,"THB" },
            { GameCurrency.IDR,"IDR" },
            { GameCurrency.INR,"INR" },
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
            decimal balance = info["userbalance"]?["tbalance"]?.Value<decimal>() ?? decimal.Zero;

            balance = this.ConvertCurrency(balance, request.Currency, null);

            return new BalanceResponse(response)
            {
                Currency = request.Currency,
                Balance = balance
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
            decimal? money = json?["transaction"]?["amount"]?.Value<decimal>();

            if (money != null)
            {
                money = this.ConvertCurrency(money.Value, request.Currency, null);
            }

            return new CheckTransferResponse(response)
            {
                TransferID = request.OrderID,
                Currency = request.Currency,
                Money = money,
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
            DateTime endDate = startDate.AddMinutes(30);

            if (endDate > DateTime.Now.AddMinutes(5)) endDate = DateTime.Now.AddMinutes(-5);
            if (endDate - startDate > TimeSpan.FromDays(1))
            {
                endDate = startDate.AddDays(1);
            }

            startDate = startDate.GetTimeZone(0).AddMinutes(-60);
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

            if (response != GameResultCode.Success) return new OrderResult(response);

            OrderResult result = new OrderResult(response)
            {
                data = new List<OrderData>()
            };

            foreach (JObject item in JObject.Parse(response)?["data"]?.Value<JArray>() ?? new JArray())
            {
                foreach (var game in item["games"]?.Value<JArray>() ?? new JArray())
                {
                    decimal reward = game["payout"]?.Value<decimal>() ?? decimal.Zero,
                        betMoney = game["wager"]?.Value<decimal>() ?? decimal.Zero,
                        money = reward - betMoney;

                    string status = game["status"]?.Value<string>() ?? string.Empty;
                    GameAPIOrderStatus orderStatus = GameAPIOrderStatus.Wait;
                    if (status == "Cancelled")
                    {
                        money = 0M;
                        orderStatus = GameAPIOrderStatus.Return;
                    }
                    else if (status == "Resolved")
                    {
                        if (money > 0)
                        {
                            orderStatus = GameAPIOrderStatus.Win;
                        }
                        else if (money < 0)
                        {
                            orderStatus = GameAPIOrderStatus.Lose;
                        }
                        else
                        {
                            orderStatus = GameAPIOrderStatus.Return;
                        }
                    }

                    result.data.Add(new OrderData
                    {
                        orderId = game["id"]?.Value<string>() ?? string.Empty,
                        currency = this.ConvertCurrency(game["currency"]?.Value<string>() ?? string.Empty),
                        playerName = game["participants"]?.Value<JArray>().FirstOrDefault()["playerId"]?.Value<string>() ?? string.Empty,
                        createTime = WebAgent.GetTimestamps(game["startedAt"]?.Value<DateTime>() ?? new DateTime(1970, 1, 1)),
                        gameCode = game["gameType"]?.Value<string>() ?? string.Empty,
                        category = "Live",
                        betMoney = betMoney,
                        betAmount = Math.Min(Math.Abs(money), betMoney),
                        money = money,
                        settleTime = WebAgent.GetTimestamps(game["settledAt"]?.Value<DateTime>() ?? new DateTime(1970, 1, 1)),
                        rawData = game.ToString(),
                        status = orderStatus,
                        hash = WebAgent.GetTimestamps(game["settledAt"]?.Value<DateTime>() ?? new DateTime(1970, 1, 1)).ToString()
                    });
                }
            }

            return result;
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

            decimal money = this.ConvertCurrency(request.Money, targetCurrency: request.Currency);

            string url = $"/api/ecashier?cCode={cCode}&ecID={this.CasinoKey}&euID={request.PlayerName}&amount={Math.Abs(money)}&eTransID={request.OrderID}&output=0";

            GameResponse response = this.Request(new GameRequest
            {
                Url = url,
                Method = APIMethod.Transfer
            });

            if (response.Code != GameResultCode.Success)
            {
                return new TransferResponse(response.Code)
                {
                    Currency = request.Currency,
                    Money = request.Money,
                    Message = response.Message,
                    Status = response.Code == GameResultCode.Exception ? GameAPITransferStatus.Unknow : GameAPITransferStatus.Faild
                };
            }

            JObject? json = response ? JObject.Parse(response) : null;


            decimal? balance = json?["transfer"]?["balance"]?.Value<decimal>();

            return new TransferResponse(response.Code)
            {
                Currency = request.Currency,
                Money = request.Money,
                OrderID = request.OrderID,
                Balance = balance,
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


        internal override HttpResult RequestAPI(GameRequest request)
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
                    result = NetAgent.PostAsync(url, (string)request.Data, Encoding.UTF8, request.Option).Result;
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
            public string? entry { get; set; }
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

        class orderResponse
        {
            public order[] data { get; set; }
        }

        class order
        {

        }

        #endregion
    }
}
