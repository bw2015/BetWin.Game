using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace BetWin.Game.API.Handlers
{
    [Description("OB电竞")]
    public class OBESport : GameBase
    {
        [Description("网关")]
        public string Gateway { get; set; }

        [Description("密钥")]
        public string secret_key { get; set; }

        [Description("商户号")]
        public string merchant { get; set; }

        public override int CollectDelay => 10 * 1000;

        public OBESport(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>
        {
            {GameLanguage.CHN,"cn" },
            {GameLanguage.THN,"zh" },
            {GameLanguage.ENG,"en" },
            {GameLanguage.VI,"vi" },
            {GameLanguage.TH,"th" },
            {GameLanguage.JA,"jp" },
            {GameLanguage.PT,"pt" },
            {GameLanguage.RU,"ru" },
            {GameLanguage.IT,"it" },
            {GameLanguage.DE,"de" },
            {GameLanguage.FR,"fr" },
            {GameLanguage.KO,"ko" },
            {GameLanguage.ES,"es" }
        };


        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>
        {
            {GameCurrency.CNY,"1" },
            {GameCurrency.USD,"2"},
            {GameCurrency.HKD,"3" },
            {GameCurrency.KVND,"4"},
            {GameCurrency.SGD,"5"},
            {GameCurrency.GBP,"6"},
            {GameCurrency.EUR,"7"},
            {GameCurrency.TWD,"9"},
            {GameCurrency.JPY,"10"},
            {GameCurrency.PHP,"11" },
            {GameCurrency.KRW,"12" },
            {GameCurrency.AUD,"13" },
            {GameCurrency.CAD,"14" },
            {GameCurrency.AED,"15" },
            {GameCurrency.MOP,"16" },
            {GameCurrency.DZD,"17" },
            {GameCurrency.OMR,"18" },
            {GameCurrency.EGP,"19" },
            {GameCurrency.RUB,"20" },
            {GameCurrency.MYR,"22" },
            {GameCurrency.VND,"23" },
            {GameCurrency.INR,"24" },
            {GameCurrency.IDR,"25" },
            {GameCurrency.THB,"26" },
            {GameCurrency.BND,"27" },
        };


        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"username",request.PlayerName },
                {"merchant",this.merchant },
                {"time",WebAgent.GetTimestamps()/1000L },
            };
            data["sign"] = GetSign(data);

            string balance = this.Post<response<string>>(APIMethod.Balance, "/api/fund/getBalance", data, out GameResultCode code);
            return new BalanceResponse(code)
            {
                Balance = decimal.TryParse(balance, out decimal userBalance) ? userBalance : 0
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"merOrderId",request.OrderID },
                {"merchant",this.merchant },
                {"time",WebAgent.GetTimestamps()/1000L },
            };
            data["sign"] = GetSign(data);
            string result = this.Post<response<string>>(APIMethod.CheckTransfer, "/api/fund/transferQuery", data, out GameResultCode code);
            if (code != GameResultCode.Success)
            {
                return new CheckTransferResponse(code)
                {
                    Status = code == GameResultCode.Exception ? GameAPITransferStatus.Unknow : GameAPITransferStatus.Faild
                };
            }
            JObject obj = JObject.Parse(result);
            int status = obj.Value<int>("data");
            return new CheckTransferResponse(code)
            {
                Money = 0,
                Status = status switch
                {
                    3 => GameAPITransferStatus.Success,
                    2 => GameAPITransferStatus.Faild,
                    _ => GameAPITransferStatus.Unknow
                }
            };
        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            // 1.只能查询当前时间30天前至当前时间区间
            // 2.查询截至时间为当前时间5分钟前
            // 3.每次查询时间区间为30分钟以内


            long now = WebAgent.GetTimestamp(DateTime.Now.AddMinutes(-5.5)),
                beginTime = WebAgent.GetTimestamp(DateTime.Now.AddDays(-7)),
               startTime = request.StartTime / 1000;

            // 初始值
            if (startTime < beginTime)
            {
                startTime = beginTime;
            }
            long endTime = Math.Min(now, startTime + 30 * 60);
            if (startTime > endTime) startTime = endTime - 60;

            long orderId = 0;
            List<OrderData> list = new List<OrderData>();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"merchant",this.merchant },
                {"start_time",startTime },
                {"end_time",endTime },
                {"last_order_id",orderId },
                {"page_size",1000 },
                {"agency",false },
            };
            data["sign"] = GetOrderSign(data);
            GameResultCode code;
            while (true)
            {
                data["last_order_id"] = orderId;
                orderResponse result = this.Post<orderResponse>(APIMethod.GetOrder, "/v2/pull/order/queryScroll", data, out code);
                if (code != GameResultCode.Success) break;

                foreach (order item in result.bet)
                {
                    ESportData esport = new ESportData()
                    {
                        betId = item.market_id,
                        content = item.odd_name,
                        gameId = item.game_id,
                        leagueId = item.tournament_id,
                        matchId = item.match_id,
                        odds = item.odd,
                        oddsType = GameAPIOddsType.EU
                    };
                    GameAPIOrderStatus status = item.bet_status switch
                    {
                        3 => GameAPIOrderStatus.Wait,
                        5 => GameAPIOrderStatus.Win,
                        8 => GameAPIOrderStatus.Win,
                        6 => GameAPIOrderStatus.Lose,
                        9 => GameAPIOrderStatus.Lose,
                        _ => GameAPIOrderStatus.Return,
                    };
                    decimal amount = item.bet_amount;
                    decimal money = 0;
                    if (status == GameAPIOrderStatus.Win || status == GameAPIOrderStatus.Lose)
                    {
                        money = item.win_amount - amount;
                    }
                    string rawData = item.ToJson();
                    list.Add(new OrderData
                    {
                        betAmount = status == GameAPIOrderStatus.Return ? 0 : Math.Min(Math.Abs(money), amount),
                        betMoney = amount,
                        category = esport.Type,
                        createTime = item.bet_time,
                        currency = ConvertCurrency(item.currency_code.ToString()),
                        gameCode = esport.gameId,
                        money = money,
                        orderId = item.id,
                        playerName = item.member_account,
                        rawData = rawData,
                        hash = string.Concat(item.id, ":", item.update_time),
                        settleTime = item.settle_time * 1000L,
                        status = status,
                        data = esport
                    });
                }

                if (result.bet.Length < 1000) break;
                orderId = result.lastOrderID;
            }

            return new OrderResult(code)
            {
                data = list,
                startTime = startTime * 1000L,
                endTime = endTime * 1000L
            };
        }

        public override LoginResponse Login(LoginModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"username",request.PlayerName },
                {"password",request.PlayerPassword },
                {"merchant",this.merchant },
                {"time",  WebAgent.GetTimestamps()/1000L},
                {"client_ip",WebAgent.ConvertIPv4(request.clientIp) }
            };
            data["sign"] = GetSign(data);

            login login = this.Post<response<login>>(APIMethod.Login, "/api/v2/member/login", data, out GameResultCode code);
            if (code != GameResultCode.Success) return new LoginResponse(code);

            return new LoginResponse(code)
            {
                Method = LoginMethod.Redirect,
                Url = request.Platform.IsMobile() ? login.h5 : login.pc
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            return new LogoutResponse(GameResultCode.NotSupport)
            {
                PlayerName = request.PlayerName
            };
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            string username = this.CreateUserName(request.Prefix, request.UserName, 15);
            string password = Guid.NewGuid().ToString("N")[..8];

            Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"username",username },
                    {"password",password},
                    {"tester",0 },
                    {"merchant",this.merchant },
                    {"time" , WebAgent.GetTimestamps()/1000L},
                    {"currency_code",this.Currencies.Get(request.Currency) },
                };
            data["sign"] = GetSign(data);
            response<string> res = this.Post<response<string>>(APIMethod.Register, "/api/member/register", data, out GameResultCode code);
            if (code == GameResultCode.Success)
            {
                return new RegisterResponse(GameResultCode.Success)
                {
                    PlayerName = username,
                    Password = password
                };
            }
            return new RegisterResponse(code);
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            List<string> transfers = new List<string>();
            string md5 = request.OrderID.toMD5();
            Regex regex = new Regex(@"\d");
            if (!regex.IsMatch(md5)) return new TransferResponse(GameResultCode.TransferInvalid);
            while (true)
            {
                foreach (Match match in regex.Matches(md5))
                {
                    transfers.Add(match.Value);
                    if (transfers.Count >= 20) break;
                }
                if (transfers.Count >= 20) break;
            }

            //转账订单号（20~32位）
            string transferId = string.Join(string.Empty, transfers);
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"amount",Math.Abs(request.Money) },
                {"key",this.secret_key },
                // 这里的原因，导致不能用OrderBy排序
                {"merOrderId",transferId },
                {"merchant",this.merchant },
                {"time",WebAgent.GetTimestamps() / 1000L },
                {"type",request.Money>0?1:2 },
                {"username",request.PlayerName },
            };
            data["sign"] = data.ToQueryString().toMD5().ToLower();
            data.Remove("key");

            string result = this.Post<response<string>>(APIMethod.Transfer, "/api/fund/transfer", data, out GameResultCode code);

            if (code != GameResultCode.Success)
            {
                return new TransferResponse(code)
                {
                    OrderID = request.OrderID,
                    TransferID = transferId,
                    Money = request.Money,
                    Currency = request.Currency,
                    Status = code == GameResultCode.Exception ? GameAPITransferStatus.Unknow : GameAPITransferStatus.Faild
                };
            }

            return new TransferResponse(code)
            {
                Money = Math.Abs(request.Money),
                OrderID = request.OrderID,
                TransferID = transferId,
                PlayerName = request.PlayerName,
                Status = GameAPITransferStatus.Success
            };
        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            try
            {
                JObject obj = JObject.Parse(result);
                if (obj == null)
                {
                    message = "null";
                    return GameResultCode.Exception;
                }
                bool? success = obj["status"]?.Value<bool>();
                if (success == null)
                {
                    message = "null";
                    return GameResultCode.Exception;
                }

                if (success.Value)
                {
                    message = "success";
                    return GameResultCode.Success;
                }
                else
                {
                    message = obj["data"]?.Value<string>() ?? string.Empty;
                    return message switch
                    {
                        "succeed" => GameResultCode.Success,
                        "missing sign" => GameResultCode.SignInvalid,
                        "wrong signature" => GameResultCode.SignInvalid,
                        "merchant inactivated" => GameResultCode.SiteLock,
                        "merchant unsupported" => GameResultCode.SiteLock,
                        "merchant not found" => GameResultCode.NoMerchant,
                        "illegal merchant" => GameResultCode.ParameterInvalid,
                        "illegal time" => GameResultCode.ParameterInvalid,
                        "illegal password" => GameResultCode.ParameterInvalid,
                        "illegal merOrderId" => GameResultCode.ParameterInvalid,
                        "data error" => GameResultCode.ParameterInvalid,
                        "illegal username" => GameResultCode.PlayerNameInvalid,
                        "username or password is wrong" => GameResultCode.PlayerNameInvalid,
                        "illegal type" => GameResultCode.TransferInvalid,
                        "betLimit not a positive integer" => GameResultCode.MoneyInvalid,
                        "the amount must be greater than or equal to 1" => GameResultCode.MoneyInvalid,
                        "the username is already registered" => GameResultCode.DuplicatePlayerName,
                        "login prohibited" => GameResultCode.LoginFaild,
                        "username not exist" => GameResultCode.NoPlayer,
                        "method not allowed" => GameResultCode.RequestInvalid,
                        "balance not enough" => GameResultCode.NoBalance,
                        "merOrderId duplicate" => GameResultCode.TransferDuplicate,
                        "processing" => GameResultCode.TransferProgress,
                        "can only query bets made five minutes ago" => GameResultCode.TimeOverflow,
                        "the query range of bet slips does not exceed 30 minutes" => GameResultCode.TimeOverflow,
                        "illegal start_time" => GameResultCode.ParameterInvalid,
                        _ => GameResultCode.Error
                    };
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return GameResultCode.Exception;
            }
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpClientResponse response = client.Get(request.Url + "?" + request.Data, new Dictionary<string, string>()
                {
                     { "Accept-Encoding", "gzip" }
                });
                return response;
            }
        }

        private T Post<T>(APIMethod method, string path, Dictionary<string, object> data, out GameResultCode code, string? gateway = null) where T : class
        {
            gateway ??= this.Gateway;
            GameRequest request = new GameRequest()
            {
                Method = method,
                Url = $"{gateway}{path}",
                Data = data.ToQueryString(true)
            };
            GameResponse response = this.Request(request);
            try
            {
                code = response.Code;
                if (code == GameResultCode.Success) return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch
            {
                code = GameResultCode.Exception;
            }
            return default;

        }

        #region ========  工具方法  ========

        private string GetSign(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("key")) data.Add("key", this.secret_key);
            string value = data.OrderBy(c => c.Key).ToQueryString();
            data.Remove("key");
            return value.toMD5().ToLower();
        }

        private string GetOrderSign(Dictionary<string, object> data)
        {
            string random = Guid.NewGuid().ToString("N")[..8];
            string sign = GetSign(data);
            return $"{random[..2]}{sign[..9]}{random.Substring(2, 2)}{sign.Substring(9, 8)}{random.Substring(4, 2)}{sign[17..]}{random[6..]}";
        }

        #endregion

        #region ========  实体类  ========

        class orderResponse
        {
            public bool status { get; set; }

            public string data { get; set; }

            public order[] bet { get; set; }

            public int total { get; set; }

            public long lastOrderID { get; set; }

            public int pageSize { get; set; }
        }

        class response<T> where T : class
        {
            public bool status { get; set; }

            public T data { get; set; }

            public static implicit operator T(response<T> response)
            {
                if (response == null) return null;
                return response.data;
            }
        }

        class login
        {
            public string h5 { get; set; }

            public string pc { get; set; }

            public string token { get; set; }
        }

        class order
        {
            /// <summary>
            /// 订单号
            /// </summary>
            public string id { get; set; }

            /// <summary>
            ///  用户ID
            /// </summary>
            public long member_id { get; set; }

            /// <summary>
            /// 用户账户
            /// </summary>
            public string member_account { get; set; }

            /// <summary>
            /// 商户ID
            /// </summary>
            public long merchant_id { get; set; }

            /// <summary>
            /// 商户账号
            /// </summary>
            public string merchant_account { get; set; }

            /// <summary>
            /// 父商户ID
            /// </summary>
            public long parent_merchant_id { get; set; }

            /// <summary>
            /// 父商户账号
            /// </summary>
            public string parent_merchant_account { get; set; }

            /// <summary>
            /// 是否测试账户
            /// </summary>
            public int tester { get; set; }

            /// <summary>
            ///注单类型
            ///1-普通注单   2-串关注单  3-局内串关   4-复合玩法
            /// </summary>
            public int order_type { get; set; }

            /// <summary>
            /// 串关类型
            /// 1普通注单 2:2串1 3:3串1.
            /// </summary>
            public int parley_type { get; set; }

            /// <summary>
            /// 游戏ID
            /// </summary>
            public string game_id { get; set; }

            /// <summary>
            /// 联赛ID
            /// </summary>
            public string tournament_id { get; set; }

            /// <summary>
            /// 赛事ID
            /// </summary>
            public string match_id { get; set; }

            /// <summary>
            /// 赛事类型    1-正常
            /// </summary>
            public int match_type { get; set; }

            /// <summary>
            /// 盘口ID
            /// </summary>
            public string market_id { get; set; }

            /// <summary>
            /// 盘口中文名称
            /// </summary>
            public string market_cn_name { get; set; }

            /// <summary>
            /// 队伍名称，主客队用, 拼接
            /// </summary>
            public string team_names { get; set; }

            /// <summary>
            /// 投注项ID
            /// </summary>
            public string odd_id { get; set; }

            /// <summary>
            /// 投注项名称
            /// </summary>
            public string odd_name { get; set; }

            /// <summary>
            /// 第几局
            /// </summary>
            public int round { get; set; }

            /// <summary>
            /// 赔率
            /// </summary>
            public decimal odd { get; set; }

            /// <summary>
            /// 投注金额（元）
            /// </summary>
            public decimal bet_amount { get; set; }

            /// <summary>
            /// 派彩金额（元）
            /// </summary>
            public decimal win_amount { get; set; }

            /// <summary>
            /// 赛事阶段    1-初盘    2-滚球
            /// </summary>
            public int is_live { get; set; }

            /// <summary>
            /// 注单状态
            /// 3-待结算
            /// 4-已取消
            /// 5-赢(已中奖)
            /// 6-输(未中奖)
            /// 7-已撤销
            /// 8-赢半
            /// 9-输半
            /// 10-走水
            /// </summary>
            public int bet_status { get; set; }

            /// <summary>
            /// 确认方式
            /// 1-自动确认 2-手动待确认  3-手动确认 4-手动拒绝
            /// </summary>
            public int confirm_type { get; set; }

            /// <summary>
            /// 投注时间（毫秒）
            /// </summary>
            public long bet_time { get; set; }

            /// <summary>
            /// 结算时间（秒）
            /// </summary>
            public long settle_time { get; set; }

            /// <summary>
            /// 赛事开始时间（秒）
            /// </summary>
            public long match_start_time { get; set; }

            /// <summary>
            /// 修改时间（秒）
            /// </summary>
            public long update_time { get; set; }

            /// <summary>
            /// 结算次数
            /// </summary>
            public int settle_count { get; set; }

            /// <summary>
            /// 战队id，主客队id用, 拼接
            /// </summary>
            public string team_id { get; set; }

            /// <summary>
            /// 投注ip
            /// </summary>
            public string bet_ip { get; set; }

            /// <summary>
            /// 设备 1-PC 2-H5    3-Android   4-IOS
            /// </summary>
            public int device { get; set; }

            /// <summary>
            /// 队伍中文名称，主客队用,拼接
            /// </summary>
            public string team_cn_names { get; set; }

            /// <summary>
            /// 队伍英文名称，主客队用,拼接
            /// </summary>
            public string team_en_names { get; set; }

            /// <summary>
            /// 基准比分
            /// </summary>
            public string score_benchmark { get; set; }

            /// <summary>
            /// 币种编码
            /// </summary>
            public int currency_code { get; set; }

            /// <summary>
            /// 币种汇率
            /// </summary>
            public decimal exchange_rate { get; set; }
        }


        #endregion
    }
}
