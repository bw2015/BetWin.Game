using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace BetWin.Game.API.Handlers
{
    /// <summary>
    /// SA真人
    /// SA Gaming 後台 及 API 的時區都為 UTC+8
    /// </summary>
    public class SALive : GameBase
    {
        #region ======== 参数配置 ========

        [Description("网关")]
        public string gateway { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Description("大厅编号")]
        public string merId { get; set; }

        [Description("SecretKey")]
        public string secretkey { get; set; }

        [Description("MD5Key")]
        public string md5Key { get; set; }

        [Description("DesKey")]
        public string encryptKey { get; set; }

        [Description("游戏地址")]
        public string loginUrl { get; set; }

        [Description("lobbyCode")]
        public string lobbyCode { get; set; }

        #endregion


        public SALive(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<LanguageType, string> Languages => new Dictionary<LanguageType, string>()
        {
            {LanguageType.ENG,"en_US" },
            {LanguageType.ES,"es" },
            {LanguageType.HI,"hi" },
            {LanguageType.IND,"id" },
            {LanguageType.JA,"jp" },
            {LanguageType.PT,"pt" },
            {LanguageType.MY,"ms" },
            {LanguageType.TH,"th" },
            {LanguageType.VI,"vn" },
            {LanguageType.CHN,"zh_CN" },
            {LanguageType.THN,"zh_TW" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>()
        {
            {GameCurrency.AED,"AED" },
            {GameCurrency.AMD,"AMD" },
            {GameCurrency.ARS,"ARS" },
            {GameCurrency.AUD,"AUD" },
            {GameCurrency.AZN,"AZN" },
            {GameCurrency.BDT,"BDT" },
            {GameCurrency.BND,"BND" },
            {GameCurrency.BRL,"BRL" },
            {GameCurrency.BYN,"BYN" },

            {GameCurrency.CAD,"CAD" },
            {GameCurrency.CHF,"CHF" },
            {GameCurrency.CLP,"CLP" },
            {GameCurrency.CZK,"CZK" },
            {GameCurrency.DKK,"DKK" },
            {GameCurrency.EUR,"EUR" },
            {GameCurrency.GBP,"GBP" },
            {GameCurrency.GEL,"GEL" },
            {GameCurrency.GHS,"GHS" },
            {GameCurrency.HTG,"HTG" },
            {GameCurrency.HUF,"HUF" },

            {GameCurrency.INR,"INR" },
            {GameCurrency.IQD,"IQD" },
            {GameCurrency.JPY,"JPY" },
            {GameCurrency.KES,"KES" },

            {GameCurrency.KGS,"KGS" },
            {GameCurrency.KZT,"KZT" },
            {GameCurrency.LKR,"LKR" },
            {GameCurrency.MDL,"MDL" },
            {GameCurrency.MXN,"MXN" },
            {GameCurrency.MYR,"MYR" },

            {GameCurrency.BTC,"mXBT" },

            {GameCurrency.NAD,"NAD" },
            {GameCurrency.NGN,"NGN" },
            {GameCurrency.NOK,"NOK" },
            {GameCurrency.NZD,"NZD" },
            {GameCurrency.PEN,"PEN" },
            {GameCurrency.PHP,"PHP" },
            {GameCurrency.PKR,"PKR" },
            {GameCurrency.PLN,"PLN" },
            {GameCurrency.RUB,"RUB" },

            {GameCurrency.SEK,"SEK" },
            {GameCurrency.SGD,"SGD" },
            {GameCurrency.THB,"THB" },
            {GameCurrency.TND,"TND" },
            {GameCurrency.TMT,"TMT" },
            {GameCurrency.TRY,"TRY" },

            {GameCurrency.TWD,"TWD" },
            {GameCurrency.UAH,"UAH" },
            {GameCurrency.USD,"USD" },
            {GameCurrency.USDT,"USDT" },
            {GameCurrency.VES,"VES" },
            {GameCurrency.XAF,"XAF" },
            {GameCurrency.XOF,"XOF" },
            {GameCurrency.ZAR,"ZAR" },
            {GameCurrency.ZMW,"ZMW" },

            {GameCurrency.CDF,"CDF2" },
            {GameCurrency.COP,"COP2" },
            {GameCurrency.IDR,"IDR2" },
            {GameCurrency.IRR,"IRR2" },
            {GameCurrency.LAK,"LAK2" },

            {GameCurrency.MMK,"MMK2" },
            {GameCurrency.TZS,"TZS2" },
            {GameCurrency.UGX,"UGX2" },
            {GameCurrency.UZS,"UZS2" },
            {GameCurrency.VND,"VND2" },

            {GameCurrency.KIDR,"IDR" },
            {GameCurrency.KKHR,"KHR" },
            {GameCurrency.KLAK,"LAK" },
            {GameCurrency.KMMK,"MMK" },
            {GameCurrency.KVND,"VND" }
        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "method","GetUserStatusDV" },
                {"Username",request.PlayerName }
            };
            var response = this.Request(new GameRequest()
            {
                Method = APIMethod.Balance,
                Data = data
            });
            if (response != GameResultCode.Success)
            {
                return new BalanceResponse(response)
                {
                    Message = response.Message
                };
            }

            return new BalanceResponse(GameResultCode.Success)
            {
                Currency = request.Currency,
                Balance = decimal.Parse(XElement.Parse(response).Element("Balance").Value)
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
           {
               {"method","CheckOrderId" },
               {"OrderId",request.OrderID }
           };

            var response = this.Request(new GameRequest()
            {
                Data = data
            });
            if (response != GameResultCode.Success)
            {
                return new CheckTransferResponse(response)
                {
                    Status = response.Code switch
                    {
                        GameResultCode.Exception => GameAPITransferStatus.Unknow,
                        _ => GameAPITransferStatus.Faild
                    }
                };
            }
            XElement root = XElement.Parse(response);

            return new CheckTransferResponse(response)
            {
                Currency = request.Currency,
                Status = root.Element("isExist").Value == "true" ? GameAPITransferStatus.Success : GameAPITransferStatus.Faild,
                TransferID = request.OrderID
            };
        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            // 指定日子(中午12:00:00至下一天上午11:59:59)的下注信息
            DateTime date = WebAgent.GetTimestamps(request.StartTime);
            if (date.Hour < 12) date = date.AddDays(-1);

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"method","GetAllBetDetailsDV" },
                {"Date",date.Date.ToString("yyyy-MM-dd") }
            };
            var response = this.Request(new GameRequest()
            {
                Method = APIMethod.GetOrder,
                Data = data
            });

            if (response != GameResultCode.Success)
            {
                return new OrderResult(response);
            }

            OrderResult result = new OrderResult(response)
            {
                data = new List<OrderData>()
            };

            XElement root = XElement.Parse(response);
            foreach (XElement item in root.Element("BetDetailList").Elements())
            {
                decimal money = decimal.Parse(item.Element("ResultAmount").Value);
                GameAPIOrderStatus status = GameAPIOrderStatus.Wait;
                if (money > 0)
                {
                    status = GameAPIOrderStatus.Win;
                }
                else if (money < 0)
                {
                    status = GameAPIOrderStatus.Lose;
                }
                else
                {
                    status = GameAPIOrderStatus.Return;
                }
                result.data.Add(new OrderData()
                {
                    orderId = item.Element("BetID").Value,
                    playerName = item.Element("Username").Value,
                    category = "Live",

                    createTime = WebAgent.GetTimestamps(DateTime.Parse(item.Element("BetTime").Value)),
                    settleTime = WebAgent.GetTimestamps(DateTime.Parse(item.Element("PayoutTime").Value)),

                    gameCode = item.Element("HostID").Value,

                    betMoney = decimal.Parse(item.Element("BetAmount").Value),
                    betAmount = decimal.Parse(item.Element("Rolling").Value),
                    money = money,

                    status = status,

                    hash = $"{item.Element("BetID").Value}:{status}",
                    rawData = item.ToJson()
                });
            }

            return result;
        }

        public override LoginResponse Login(LoginModel request)
        {
            if (!this.Currencies.ContainsKey(request.Currency)) return new LoginResponse(GameResultCode.CurrencyInvalid);

            Dictionary<string, string> data = new Dictionary<string, string>()
           {
                {"method","LoginRequest" },
                {"Username", request.PlayerName },
                {"CurrencyType", this.Currencies.Get(request.Currency) ?? string.Empty }
           };

            var response = this.Request(new GameRequest()
            {
                Method = APIMethod.Login,
                Data = data
            });
            string? token = null;
            if (response == GameResultCode.Success)
            {
                token = XElement.Parse(response).Element("Token").Value;
            }

            return new LoginResponse(response)
            {
                Method = LoginMethod.Redirect,
                Url = $"{this.loginUrl}?username={request.PlayerName}&token={token}&lobby={this.lobbyCode}&lang={this.Languages.Get(request.Language)}",
                Message = response.Message
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"method", "RegUserInfo" },
                {"Username", request.UserName },
                {"CurrencyType", this.Currencies.Get(request.Currency) ?? string.Empty }
            };
            var response = this.Request(new GameRequest
            {
                Method = APIMethod.Register,
                Data = data
            });

            string? playerName = null;
            if (response == GameResultCode.Success)
            {
                playerName = XElement.Parse(response).Element("Username").Value;
            }

            return new RegisterResponse(response)
            {
                PlayerName = playerName ?? request.UserName,
                Message = response.Message
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            if (!Regex.IsMatch(request.OrderID, @"^(IN|OUT)\d{12}\w+$")) return new TransferResponse(GameResultCode.TransferInvalid);

            string orderId = request.OrderID;
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"method",request.Money > 0 ? "CreditBalanceDV" : "DebitBalanceDV" },
                {"Username",request.PlayerName },
                {"OrderId", orderId }
            };
            if (request.Money > 0)
            {
                data.Add("CreditAmount", Math.Abs(request.Money).ToString("0.00"));
            }
            else
            {
                data.Add("DebitAmount", Math.Abs(request.Money).ToString("0.00"));
            }

            var response = this.Request(new GameRequest()
            {
                Method = APIMethod.Transfer,
                Data = data
            });
            if (response != GameResultCode.Success && response != GameResultCode.Exception)
            {
                return new TransferResponse(response)
                {
                    Status = GameAPITransferStatus.Faild,
                    Message = response.Message
                };
            }

            XElement root = XElement.Parse(response);
            return new TransferResponse(response)
            {
                Status = response.Code switch
                {
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    _ => GameAPITransferStatus.Faild
                },
                OrderID = request.OrderID,
                TransferID = root.Element("OrderId").Value,
                Currency = request.Currency,
                Money = decimal.Parse(root.Element(request.Money > 0 ? "CreditAmount" : "DebitAmount").Value),
                Balance = decimal.Parse(root.Element("Balance").Value)
            };

        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            XElement root = XElement.Parse(result);

            string errorMsgId = root.Element("ErrorMsgId").Value;
            message = root.Element("ErrorMsg").Value;

            return errorMsgId switch
            {
                "0" => GameResultCode.Success,
                "100" => GameResultCode.PlayerNameInvalid,
                "101" => GameResultCode.PlayerLocked,
                "102" => GameResultCode.SignInvalid,
                "104" => GameResultCode.Maintenance,
                "105" => GameResultCode.ClientInvalid,
                "106" => GameResultCode.SystemBuzy,
                "107" => GameResultCode.PlayerNameInvalid,
                "108" => GameResultCode.PlayerNameInvalid,
                // User not online 用户离线
                "110" => GameResultCode.PlayerLocked,
                "111" => GameResultCode.TimeOverflow,
                "112" => GameResultCode.SystemBuzy,

                "113" => GameResultCode.DuplicatePlayerName,
                "114" => GameResultCode.CurrencyInvalid,
                "116" => GameResultCode.NoPlayer,
                "120" => GameResultCode.NoBalance,
                "121" => GameResultCode.NoBalance,
                "122" => GameResultCode.TransferDuplicate,
                //Kick user fail 踢出用户失败
                "125" => GameResultCode.PlayerLocked,
                "127" => GameResultCode.TransferInvalid,
                "128" => GameResultCode.SignInvalid,
                "129" => GameResultCode.Maintenance,
                "130" => GameResultCode.PlayerLocked,
                "132" => GameResultCode.SignInvalid,
                // Create user failed 建立帐户失败
                "133" => GameResultCode.DuplicatePlayerName,
                "135" => GameResultCode.GameClose,

                "136" => GameResultCode.NoBalance,
                "137" => GameResultCode.ConfigInvalid,
                "138" => GameResultCode.ConfigInvalid,
                "142" => GameResultCode.ParameterInvalid,
                "144" => GameResultCode.ParameterInvalid,
                "145" => GameResultCode.MoneyInvalid,

                "146" => GameResultCode.SiteLock,
                "151" => GameResultCode.LoginFaild,
                "152" => GameResultCode.OrderNotFound,

                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            Dictionary<string, string>? data = (Dictionary<string, string>)request.Data;
            if (data == null) return new HttpResult(new Exception("参数错误"), this.gateway);

            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            data.Add("Key", this.secretkey);
            data.Add("Time", time);

            string qs = data.ToQueryString();
            string q = HttpUtility.UrlEncode(this.DESEncrypt(qs));
            string s = this.BuildMD5($"{qs}{this.md5Key}{time}{this.secretkey}");

            return NetAgent.PostAsync(this.gateway, $"q={q}&s={s}", Encoding.UTF8).Result;
        }

        #region ========  工具方法  ========

        protected byte[] EncryptKey => Encoding.ASCII.GetBytes(this.encryptKey);

        private string DESEncrypt(string inString)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, new DESCryptoServiceProvider().CreateEncryptor(this.EncryptKey, this.EncryptKey),
                CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cs);
            sw.Write(inString);
            sw.Flush();
            cs.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        public string BuildMD5(string inString)
        {
            byte[] hashed = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(inString));
            StringBuilder sb = new StringBuilder(hashed.Length * 2);
            for (int i = 0; i < hashed.Length; i++)
            {
                sb.Append(hashed[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}
