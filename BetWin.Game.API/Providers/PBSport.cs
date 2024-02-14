using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace BetWin.Game.API.Providers
{
    [Description("平博体育")]
    internal class PBSport : GameBase
    {
        [Description("网关")]
        public string Gateway { get; set; }

        [Description("AccessToken")]
        public string AccessToken { get; set; }

        [Description("EncryptKey")]
        public string EncryptKey { get; set; }

        [Description("AgentCode")]
        public string AgentCode { get; set; }

        public PBSport(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>()
        {
            {GameLanguage.ENG,"en" },
            {GameLanguage.CHN,"zh-cn" },
            {GameLanguage.THN,"zh-tw" },
            {GameLanguage.IND,"id" },
            {GameLanguage.VI,"vi" },
            {GameLanguage.JA,"ja" },
            {GameLanguage.KO,"ko" },
            {GameLanguage.TH,"th" },
            {GameLanguage.FR,"fr" },
            {GameLanguage.DE,"de" },
            {GameLanguage.ES,"es" },
            {GameLanguage.PT,"pt" },
            {GameLanguage.RU,"ru" },
            {GameLanguage.TR,"tr" },
            {GameLanguage.HI,"hi" },
            {GameLanguage.AR,"hy" },
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>
        {

        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                {"userCode",request.PlayerName }
            };
            balance? balance = this.Post<balance>(APIMethod.Balance, data, out GameResultCode code, out string message);
            return new BalanceResponse(code)
            {
                Balance = balance?.availableBalance ?? decimal.Zero,
                Message = message
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"transactionId",request.OrderID }
            };
            checkTransfer? checkTransfer = this.Post<checkTransfer>(APIMethod.CheckTransfer, data, out GameResultCode code, out string message);
            return new CheckTransferResponse(code)
            {
                Currency = request.Currency,
                Money = checkTransfer?.amount ?? 0,
                TransferID = request.OrderID,
                Message = checkTransfer?.status ?? message,
                Status = code switch
                {
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    _ => GameAPITransferStatus.Faild
                }
            };

        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            DateTime start = WebAgent.GetTimestamps(request.StartTime).AddHours(-12);
            DateTime end = start.AddHours(1);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"dateFrom",start.ToString("yyyy-MM-dd HH:mm:ss") },
                {"dateTo",end.ToString("yyyy-MM-dd HH:mm:ss")},
            };

            order[]? orders = this.Post<order[]>(APIMethod.GetOrder, data, out GameResultCode code, out string message);
            if (code != GameResultCode.Success) return new OrderResult(code)
            {
                Message = message
            };

            return new OrderResult(code)
            {
                Message = message
            };
        }

        public override LoginResponse Login(LoginModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"userCode",request.PlayerName },
                {"locale",this.Languages.Get(request.Language) },
                {"oddsFormat","EU" },
                {"desktopView",false},
            };

            login? login = this.Post<login>(APIMethod.Login, data, out GameResultCode code, out string message);

            return new LoginResponse(code)
            {
                Url = login?.loginUrl,
                Method = LoginMethod.Redirect,
                Message = message
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            string username = CreateUserName(request.Prefix, request.UserName, 50);
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"loginId",username }
            };
            register? register = this.Post<register>(APIMethod.Register, data, out GameResultCode code, out string message);
            return new RegisterResponse(code)
            {
                PlayerName = register?.loginId ?? string.Empty,
                Message = register?.userCode ?? message,
                Password = register?.userCode ?? string.Empty
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            string url;
            if (request.Money > 0)
            {
                url = this.Gateway + "/player/deposit";
            }
            else
            {
                url = this.Gateway + "/player/withdraw";
            }

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"userCode",request.PlayerName },
                {"amount",Math.Abs(request.Money) },
                {"transactionId",request.OrderID }
            };
            transfer? transfer = this.Post<transfer>(APIMethod.Transfer, data, out GameResultCode code, out string message, url);

            return new TransferResponse(code)
            {
                Balance = transfer?.availableBalance,
                Money = transfer?.amount ?? 0,
                Message = transfer?.message,
                TransferID = request.OrderID,
                PlayerName = request.PlayerName,
                OrderID = request.OrderID,
                Currency = request.Currency,
                Status = code switch
                {
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    _ => GameAPITransferStatus.Faild
                }
            };

        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            responseBase? response = result.ToJson<responseBase>();
            message = response?.message ?? result;
            if (response == null && !result.StartsWith("["))
            {
                return GameResultCode.Exception;
            }
            return response?.code switch
            {
                "successful" => GameResultCode.Success,
                null => GameResultCode.Success,
                "103" => GameResultCode.PlayerLocked,
                "106" => GameResultCode.PlayerLocked,
                "104" => GameResultCode.NoPlayer,
                "115" => GameResultCode.NoPlayer,
                "105" => GameResultCode.PlayerNameInvalid,
                "112" => GameResultCode.PlayerNameInvalid,
                "107" => GameResultCode.NoMerchant,
                "108" => GameResultCode.NoMerchant,
                "109" => GameResultCode.NoMerchant,
                "110" => GameResultCode.NoMerchant,
                "111" => GameResultCode.DuplicatePlayerName,
                "306" => GameResultCode.ParameterInvalid,
                "308" => GameResultCode.TransferInvalid,
                "309" => GameResultCode.NoBalance,
                "310" => GameResultCode.MoneyInvalid,
                "311" => GameResultCode.MoneyInvalid,
                "401" => GameResultCode.SignInvalid,
                "403" => GameResultCode.SignInvalid,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                {"userCode",this.AgentCode },
                {"token",getToken() }
            };
            using (HttpClient client = new HttpClient())
            {
                return client.Get($"{request.Url}?{request.Data}", headers);
            }
        }

        #region ========  工具方法  ========

        private T? Post<T>(APIMethod method, Dictionary<string, object> data, out GameResultCode code, out string message, string? url = null) where T : class
        {
            url = method switch
            {
                APIMethod.Login => $"{this.Gateway}/player/login",
                APIMethod.Register => $"{this.Gateway}/player/create",
                APIMethod.Balance => $"{this.Gateway}/player/info",
                APIMethod.CheckTransfer => $"{this.Gateway}/player/depositwithdraw/status",
                APIMethod.GetOrder => $"{this.Gateway}/report/all-wagers",
                _ => url ?? string.Empty
            };

            var response = this.Request(new GameRequest()
            {
                Url = url,
                Data = data.ToQueryString(),
                Method = method
            });

            code = response.Code;
            message = response.Message;

            return response.Content.ToJson<T>();
        }


        private string getToken()
        {
            string key = this.GetType().FullName;
            string? token = this.handler?.GetData(key);
            if (string.IsNullOrWhiteSpace(token))
            {
                token = generateToken(this.AgentCode, this.AccessToken, this.EncryptKey);
                this.handler?.SaveData(key, token, TimeSpan.FromMinutes(10));
            }
            return token;
        }

        private string generateToken(string agentCode, string agentKey, string secretKey)
        {
            string timeStamp = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            string hashToken = (agentCode + timeStamp + agentKey).toMD5().ToLower();
            string tokenPayLoad = String.Format("{0}|{1}|{2}", agentCode, timeStamp, hashToken);
            return encryptAES(secretKey, tokenPayLoad);
        }

        private string encryptAES(string secretKey, string tokenPayLoad)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true, secretKey);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(tokenPayLoad);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                string encrypted = Convert.ToBase64String(output);
                return encrypted;
            }
        }


        private static string INIT_VECTOR = "RandomInitVector";

        private ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting, string secretKey)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            csp.IV = Encoding.UTF8.GetBytes(INIT_VECTOR);
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }

        #endregion

        #region ========  实体类  ========

        class responseBase
        {
            public string? code { get; set; }

            public string? message { get; set; }
        }

        /// <summary>
        /// 登录
        /// </summary>
        class login : responseBase
        {
            public string? userCode { get; set; }

            public string? loginId { get; set; }

            public string? token { get; set; }

            public string? loginUrl { get; set; }

            public DateTime? updateDate { get; set; }
        }

        class register : responseBase
        {
            public string? loginId { get; set; }

            public string? userCode { get; set; }
        }

        /// <summary>
        /// {"userCode":"341110000G","loginId":"api_peter","firstName":"","lastName":"","status":"ACTIVE","availableBalance":0.00,"outstanding":0.00,"createdDate":"2024-02-07 20:55:22","createdBy":"B2B"}
        /// </summary>
        class balance : responseBase
        {
            public string? userCode { get; set; }

            public string? loginId { get; set; }

            public string? firstName { get; set; }

            public string? lastName { get; set; }

            public string? status { get; set; }

            public string? outstanding { get; set; }

            public DateTime? createdDate { get; set; }

            public string? createdBy { get; set; }

            public decimal? availableBalance { get; set; }
        }

        /// <summary>
        /// {\"amount\":100,\"loginId\":\"api_peter\",\"userCode\":\"341110000G\",\"availableBalance\":100.00}
        /// </summary>
        class transfer : responseBase
        {
            public string? loginId { get; set; }

            public string? userCode { get; set; }

            public decimal? availableBalance { get; set; }

            public decimal? amount { get; set; }
        }

        /// <summary>
        /// {"status":"SUCCESS","userCode":"341110000G","transferType":"DEPOSIT","amount":100.00,"transferDate":"2024-02-07 22:41:34"}
        /// </summary>
        class checkTransfer : responseBase
        {
            public string? status { get; set; }

            public decimal? amount { get; set; }

            public string? userCode { get; set; }

            public string? transferType { get; set; }

            public DateTime? transferDate { get; set; }
        }

        /// <summary>
        /// {"wagerId":2527931434,"eventId":1585338015,"eventName":"Movistar R7 -vs- Estral","parentEventName":null,"headToHead":null,"wagerDateFm":"2024-02-07 22:43:51","eventDateFm":"2024-02-07 21:15:00","settleDateFm":"2024-02-07 23:33:22","resettleDateFm":null,"status":"SETTLED","homeTeam":"Movistar R7","awayTeam":"Estral","selection":"Movistar R7","handicap":0.00,"odds":1.675,"oddsFormat":1,"betType":1,"league":"英雄联盟 - LLA","leagueId":211523,"stake":10.00,"sportId":12,"sport":"电子竞技","currencyCode":"CNY","inplayScore":"","inPlay":true,"homePitcher":null,"awayPitcher":null,"homePitcherName":null,"awayPitcherName":null,"period":0,"cancellationStatus":null,"parlaySelections":[],"category":null,"toWin":6.7500000,"toRisk":10.0000000,"product":"SB","isResettle":false,"parlayMixOdds":1.6750000,"parlayFinalOdds":1.6750000,"wagerType":"single","competitors":[],"userCode":"341110000G","loginId":"api_peter","winLoss":-10.00,"turnover":10.000000000,"scores":[{"period":0,"score":"1-2"},{"period":1,"score":"0-1"},{"period":2,"score":"1-0"},{"period":3,"score":"0-1"}],"result":"LOSE","volume":6.75,"view":"D-Compact"}
        /// </summary>
        class order
        {
            /// <summary>
            /// 订单号
            /// </summary>
            public string wagerId { get; set; }


            public string? selection { get; set; }

            public string? leagueId { get; set; }

            public string league { get; set; }

            /// <summary>
            /// 比赛ID
            /// </summary>
            public string? eventId { get; set; }

            /// <summary>
            /// 赛事名称
            /// </summary>
            public string? eventName { get; set; }

            public decimal? odds { get; set; }

            public string? sportId { get; set; }

            public string sport { get; set; }

            public int? oddsFormat { get; set; }
        }

        #endregion
    }
}

