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
            throw new NotImplementedException();
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            throw new NotImplementedException();
        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            responseBase? response = result.ToJson<responseBase>();
            message = response?.message ?? result;
            return response?.code switch
            {
                "successful" => GameResultCode.Success,
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

        private T? Post<T>(APIMethod method, Dictionary<string, object> data, out GameResultCode code, out string message) where T : responseBase
        {
            string url = method switch
            {
                APIMethod.Login => $"{this.Gateway}/player/login",
                _ => throw new NotImplementedException(method.ToString())
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
            public string code { get; set; }

            public string message { get; set; }
        }

        /// <summary>
        /// 登录
        /// </summary>
        class login : responseBase
        {
            public string? loginUrl { get; set; }
        }

        #endregion
    }
}

