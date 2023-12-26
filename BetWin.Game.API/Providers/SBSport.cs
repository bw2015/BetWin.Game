using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.API.Providers
{
    [Description("沙巴体育")]
    public class SBSport : GameBase
    {

        [Description("网关")]
        public string gateway { get; set; }

        /// <summary>
        /// 厂商ID
        /// </summary>
        [Description("厂商ID")]
        public string operatorId { get; set; }

        /// <summary>
        /// 厂商识别码
        /// </summary>
        [Description("厂商识别码")]
        public string vendorId { get; set; }

        [Description("前缀")]
        public string prefix { get; set; }

        public SBSport(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>()
        {
            {GameLanguage.ENG,"en" },
            {GameLanguage.CHN,"cs" },
            {GameLanguage.THN,"ch" },
            {GameLanguage.TH,"th" },
            {GameLanguage.VI,"vn" },
            {GameLanguage.IND,"id" },
            {GameLanguage.PT,"ptbr" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>()
        {
            {GameCurrency.MYR,"2" },
            {GameCurrency.USD,"3" },
            {GameCurrency.THB,"4" },
            {GameCurrency.EUR,"6" },
            {GameCurrency.GBP,"12" },
            {GameCurrency.CNY,"13" },
            {GameCurrency.IDR,"15" },
            {GameCurrency.JPY,"32" },
            {GameCurrency.CHF,"41" },
            {GameCurrency.PHP,"42" },
            {GameCurrency.KRW,"45" },
            {GameCurrency.BND,"46" },
            {GameCurrency.MXN,"49" },
            {GameCurrency.CAD,"50" },
            {GameCurrency.VND,"51" },
            {GameCurrency.DKK,"52" },
            {GameCurrency.SEK,"53" },
            {GameCurrency.NOK,"54" },
            {GameCurrency.RUB,"55" },
            {GameCurrency.PLN,"56" },
            {GameCurrency.CZK,"57" },
            {GameCurrency.RON,"58" },
            {GameCurrency.INR,"61" },
            {GameCurrency.MMK,"70" },
            {GameCurrency.KHR,"71" },
            {GameCurrency.TRY,"73" },
            {GameCurrency.KES,"79" },
            {GameCurrency.BRL,"82" },
            {GameCurrency.AED,"90" },
            {GameCurrency.LAK,"93" },
            {GameCurrency.USDT,"96" },
            {GameCurrency.BDT,"97" },
            {GameCurrency.PKR,"121" },
            {GameCurrency.KZT,"122" },
            {GameCurrency.NPR,"123" },
            {GameCurrency.TND,"133" },
            {GameCurrency.ZMW,"134"},
            {GameCurrency.TZS,"135" }
        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"vendor_id",this.vendorId },
                {"vendor_member_ids",request.PlayerName },
                {"wallet_id",1 }
            };

            ///{\"vendor_member_id\":\"bykj_test_peter\",\"balance\":200.0,\"outstanding\":0.0,\"currency\":13,\"error_code\":0}
            response<_balance[]>? balance = this.post<response<_balance[]>>(APIMethod.Balance, "/api/CheckUserBalance/", data, out GameResultCode code, out string message);

            return new BalanceResponse(code)
            {
                Currency = this.ConvertCurrency(balance?.Data?.FirstOrDefault()?.currency ?? ""),
                Balance = balance?.Data?.FirstOrDefault()?.balance ?? decimal.Zero,
                Message = message
            };


        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"vendor_id ",this.vendorId},
                {"vendor_trans_id",request.OrderID},
                {"wallet_id",1 }
            };

            response<checkTransfer>? response = this.post<response<checkTransfer>>(APIMethod.CheckTransfer, "/api/CheckFundTransfer", data, out GameResultCode code, out string message);

            return new CheckTransferResponse(code)
            {
                Message = message,
                Currency = this.ConvertCurrency(response?.Data?.currency ?? string.Empty),
                TransferID = response?.Data?.trans_id ?? request.OrderID,
                Money = response?.Data?.amount,
                Status = code switch
                {
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    GameResultCode.SystemBuzy => GameAPITransferStatus.Unknow,
                    GameResultCode.Success => (
                        response?.Data?.status switch
                        {
                            0 => GameAPITransferStatus.Success,
                            1 => GameAPITransferStatus.Faild,
                            _ => GameAPITransferStatus.Unknow
                        }
                    ),
                    _ => GameAPITransferStatus.Faild
                }
            };
        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            var data = new Dictionary<string, object>()
            {
                {"vendor_id",this.vendorId },
                {"version_key",request.StartTime }
            };

            OrderResult result = new OrderResult(GameResultCode.Success)
            {
                startTime = request.StartTime,
                data = new List<OrderData>()
            };

            response<orderQuery>? response = this.post<response<orderQuery>>(APIMethod.GetOrder, "/api/GetBetDetail", data, out GameResultCode code, out string message);

            if (code != GameResultCode.Success)
            {
                return new OrderResult(code)
                {
                    Message = message
                };
            }

            foreach (orderDetail order in response?.Data?.BetDetails ?? Array.Empty<orderDetail>())
            {
                GameAPIOrderStatus status = order.ticket_status switch
                {
                    "waiting" => GameAPIOrderStatus.Wait,
                    "running" => GameAPIOrderStatus.Wait,
                    "lose" => GameAPIOrderStatus.Lose,
                    "half lose" => GameAPIOrderStatus.Lose,
                    "won" => GameAPIOrderStatus.Win,
                    "half won" => GameAPIOrderStatus.Win,
                    _ => GameAPIOrderStatus.Return
                };

                result.data.Add(new OrderData()
                {
                    orderId = order.trans_id,
                    playerName = order.vendor_member_id,
                    status = status,
                    createTime = WebAgent.GetTimestamps(order.transaction_time, TimeSpan.FromHours(-4)),
                    currency = this.ConvertCurrency(order.currency),
                    betMoney = order.stake,
                    money = order.winlost_amount,
                    betAmount = Math.Min(Math.Abs(order.winlost_amount), order.stake),
                    category = "Sport",
                    gameCode = order.sport_type.ToString(),
                    settleTime = order.settlement_time == null ? 0 : WebAgent.GetTimestamps(order.settlement_time.Value, TimeSpan.FromHours(-4)),
                    hash = $"{order.trans_id}:{order.settlement_time}",
                    rawData = order.ToJson()
                });
            }

            result.endTime = response?.Data?.last_version_key ?? request.StartTime;

            return result;
        }

        public override LoginResponse Login(LoginModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"vendor_id",this.vendorId},
                {"platform", request.Platform switch
                    {
                        GamePlatform.PC => 1,
                        GamePlatform.Mobile => 2,
                        _ => 1
                    }
                },
                {"vendor_member_id",request.PlayerName }
            };

            login? login = this.post<login>(APIMethod.Login, "/api/GetSabaUrl", data, out GameResultCode code, out string message);

            string? url = login?.data;
            if (url != null)
            {
                url += "&lang=" + this.Languages.Get(request.Language);
            }
            return new LoginResponse(code)
            {
                Message = message,
                Method = LoginMethod.Redirect,
                Url = url
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            string userName = this.prefix + this.CreateUserName(request.Prefix, request.UserName, 30);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"vendor_id",this.vendorId },
                {"vendor_Member_ID",userName },
                {"operatorId",this.operatorId },
                {"userName",userName },
                {"oddsType",3 },
                {"currency",this.Currencies.Get(request.Currency,"3") },
                {"maxTransfer",9999999999 },
                {"minTransfer",1 },
            };

            register? register = this.post<register>(APIMethod.Register, "/api/CreateMember", data, out GameResultCode code, out string message);

            return new RegisterResponse(code)
            {
                Message = message,
                PlayerName = userName
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"vendor_id",this.vendorId },
                {"vendor_member_id",request.PlayerName },
                {"vendor_trans_id", this.prefix + request.OrderID },
                {"amount",Math.Abs(request.Money) },
                {"currency",this.Currencies.Get(request.Currency) },
                {"direction",request.Money>0?1:0 },
                {"wallet_id",1 }
            };

            response<transfer>? response = this.post<response<transfer>>(APIMethod.Transfer, "/api/FundTransfer/", data, out GameResultCode code, out string message);

            return new TransferResponse(code)
            {
                Message = message,
                Currency = request.Currency,
                Money = request.Money,
                Balance = response?.Data?.after_amount,
                OrderID = request.OrderID,
                TransferID = data["vendor_trans_id"].ToString(),
                PlayerName = request.PlayerName,
                Status = code switch
                {
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    GameResultCode.Success => (
                        response?.Data?.status switch
                        {
                            0 => GameAPITransferStatus.Success,
                            _ => GameAPITransferStatus.Unknow
                        }
                    ),
                    _ => GameAPITransferStatus.Faild
                }
            };
        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            response? response = result.ToJson<response>();
            if (response == null)
            {
                message = result;
                return GameResultCode.Exception;
            }

            message = response.message ?? result;

            return response.error_code switch
            {
                0 => GameResultCode.Success,
                1 => GameResultCode.ParameterInvalid,
                7 => GameResultCode.ParameterInvalid,
                2 => GameResultCode.DuplicatePlayerName,
                3 => GameResultCode.NoMerchant,
                9 => GameResultCode.NoMerchant,
                5 => GameResultCode.CurrencyInvalid,
                6 => GameResultCode.CurrencyInvalid,
                10 => GameResultCode.Maintenance,
                11 => GameResultCode.SystemBuzy,
                15 => GameResultCode.PlayerLocked,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                return client.Post(request.Url, request.Data, new Dictionary<string, string>()
                {
                    {"Content-Type","application/x-www-form-urlencoded" }
                });
            }
        }

        #region ========  工具方法  ========

        private T? post<T>(APIMethod method, string path, Dictionary<string, object> data, out GameResultCode code, out string message) where T : class
        {
            string url = $"{this.gateway}{path}";
            GameResponse response = this.Request(new GameRequest()
            {
                Url = url,
                Data = data.ToQueryString(),
                Method = method
            });

            code = response.Code;
            message = response.Message;

            return response.Content.ToJson<T>();
        }

        #endregion

        #region ========  实体类  ========

        class response
        {
            public int error_code { get; set; }

            public string? message { get; set; }
        }

        class response<T> : response where T : class
        {
            public T? Data { get; set; }
        }

        class login : response
        {
            /// <summary>
            /// 登录地址
            /// </summary>
            public string? data { get; set; }
        }

        class register : response
        {

        }

        class _balance
        {
            public string vendor_member_id { get; set; }

            public decimal balance { get; set; }

            public string outstanding { get; set; }

            public string currency { get; set; }

            public int error_code { get; set; }
        }

        class transfer
        {
            public string trans_id { get; set; }

            public decimal? before_amount { get; set; }

            public decimal? after_amount { get; set; }

            public string? system_id { get; set; }

            public int status { get; set; }
        }

        class checkTransfer
        {
            public string trans_id { get; set; }

            public DateTime transfer_date { get; set; }

            public string vender_member_id { get; set; }

            public decimal? amount { get; set; }

            public string currency { get; set; }

            public decimal? before_amount { get; set; }

            public decimal? after_amount { get; set; }

            public int status { get; set; }
        }

        class orderQuery
        {
            public long last_version_key { get; set; }

            /// <summary>
            /// 订单列表
            /// </summary>
            public orderDetail[] BetDetails { get; set; }
        }

        class orderDetail
        {
            public string trans_id { get; set; }

            public string vendor_member_id { get; set; }

            public string operator_id { get; set; }

            public string league_id { get; set; }

            public language[] leaguename { get; set; }

            public int match_id { get; set; }

            public int home_id { get; set; }

            public language[] hometeamname { get; set; }

            public int away_id { get; set; }

            public language[] awayteamname { get; set; }


            public DateTime match_datetime { get; set; }

            public int sport_type { get; set; }

            public language[] sportname { get; set; }

            public int bet_type { get; set; }

            public language[] bettypename { get; set; }

            public int parlay_ref_no { get; set; }

            public decimal odds { get; set; }

            public decimal stake { get; set; }

            /// <summary>
            /// 投注时间，GTM-4 市区
            /// </summary>
            public DateTime transaction_time { get; set; }

            public string ticket_status { get; set; }

            public decimal winlost_amount { get; set; }

            public decimal after_amount { get; set; }

            public string currency { get; set; }

            public DateTime? winlost_datetime { get; set; }

            public int odds_type { get; set; }

            public string bet_team { get; set; }

            public string isLucky { get; set; }

            public decimal home_hdp { get; set; }

            public decimal away_hdp { get; set; }

            public decimal hdp { get; set; }

            public string betfrom { get; set; }


            public int islive { get; set; }

            public decimal home_score { get; set; }

            public decimal away_score { get; set; }

            /// <summary>
            /// 结算时间
            /// </summary>
            public DateTime? settlement_time { get; set; }

            public string customInfo1 { get; set; }
            public string customInfo2 { get; set; }
            public string customInfo3 { get; set; }
            public string customInfo4 { get; set; }
            public string customInfo5 { get; set; }

            public string ba_status { get; set; }

            public long version_key { get; set; }

            /// <summary>
            /// 串关内容
            /// </summary>
            public parlayData[]? ParlayData { get; set; }

            public string risklevelname { get; set; }

            public string risklevelnamecs { get; set; }
        }

        class language
        {
            public string lang { get; set; }

            public string name { get; set; }
        }

        /// <summary>
        /// 串关的内容
        /// </summary>
        class parlayData
        {
            public long parlay_id { get; set; }
            public int league_id { get; set; }
            public long match_id { get; set; }
            public int home_id { get; set; }
            public int away_id { get; set; }
            public DateTime match_datetime { get; set; }
            public double odds { get; set; }
            public int bet_type { get; set; }
            public string bet_team { get; set; }
            public int sport_type { get; set; }
            public double home_hdp { get; set; }
            public double away_hdp { get; set; }
            public double hdp { get; set; }
            public int islive { get; set; }
            public object home_score { get; set; }
            public string ticket_status { get; set; }
            public DateTime winlost_datetime { get; set; }
        }


        #endregion
    }
}
