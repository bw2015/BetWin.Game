using BetWin.Game.API.Enums;
using BetWin.Game.API.Exceptions;
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
using System.Reflection;
using System.Text;

namespace BetWin.Game.API.Providers
{
    [Description("OB体育")]
    public class OBSport : GameBase
    {
        #region ========  接口参数  ========

        [Description("网关")]
        public string gateway { get; set; }

        [Description("商户编码")]
        public string merchantCode { get; set; }

        [Description("密钥")]
        public string key { get; set; }

        [Description("语种")]
        public string language { get; set; } = "";

        #endregion

        public OBSport(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>()
        {
            { GameLanguage.CHN,"zh" },
            { GameLanguage.ENG,"en" },
            { GameLanguage.VI,"vi" },
            { GameLanguage.THN,"tw" },
            { GameLanguage.TH,"th" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>()
        {
            { GameCurrency.CNY,"1"  },
            { GameCurrency.USD,"2" },
            { GameCurrency.HKD,"3" },
            { GameCurrency.KVND,"4" },
            { GameCurrency.SGD, "5" },
            { GameCurrency.GBP,"6" },
            { GameCurrency.EUR,"7" },
            { GameCurrency.BTC,"8" },
            { GameCurrency.TWD,"9" },
            { GameCurrency.JPY,"10" },
            { GameCurrency.PHP,"11" },
            { GameCurrency.KRW,"12" },
            { GameCurrency.AUD,"13" },
            { GameCurrency.CAD,"14" },
            { GameCurrency.AED,"15" },
            { GameCurrency.MOP,"16" },
            { GameCurrency.DZD,"17" },
            { GameCurrency.OMR,"18" },
            { GameCurrency.EGP,"19" },
            { GameCurrency.RUB,"20" },
            { GameCurrency.KIDR,"21" },
            { GameCurrency.MYR,"22" },
            { GameCurrency.VND,"23" },
            { GameCurrency.INR,"24" },
            { GameCurrency.IDR,"25" },
            { GameCurrency.THB,"26" }
        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"userName",request.PlayerName },
                { nameof(merchantCode),this.merchantCode },
                {"timestamp", WebAgent.GetTimestamps() }
            };
            data.Add("signature", this.getSign(data[nameof(merchantCode)], data["userName"], data["timestamp"]));

            var balance = this.Post<balanceResponse>(APIMethod.Balance, "/api/fund/checkBalance", data, out GameResultCode code, out string message);
            if (balance == null)
            {
                return new BalanceResponse(code)
                {
                    Message = message
                };
            };

            return new BalanceResponse(code)
            {
                Balance = balance?.data?.balance ?? 0,
                Message = message
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"userName",request.PlayerName },
                { nameof(merchantCode),this.merchantCode },
                {"transferId", request.OrderID },
                {"timestamp", WebAgent.GetTimestamps() }
            };
            data.Add("signature", this.getSign(this.merchantCode, request.OrderID, data["timestamp"]));

            var response = this.Post<checkTransfer>(APIMethod.CheckTransfer, "/api/fund/getTransferRecord", data, out GameResultCode code, out string message);
            if (response == null)
            {
                return new CheckTransferResponse(code)
                {
                    Message = message
                };
            }

            return new CheckTransferResponse(code)
            {
                Money = response?.data?.amount ?? 0,
                TransferID = response?.data?.transferId ?? request.OrderID,
                Message = message,
                Status = (code, response?.data?.status) switch
                {
                    (GameResultCode.Success, 1) => GameAPITransferStatus.Success,
                    (GameResultCode.Success, 0) => GameAPITransferStatus.Faild,
                    (GameResultCode.Exception, null) => GameAPITransferStatus.Unknow,
                    _ => GameAPITransferStatus.Faild
                }
            };
        }



        public override OrderResult GetOrder(QueryOrderModel request)
        {
            int pageNum = 0;
            Dictionary<string, GameCurrency> currency = this.Currencies.ToDictionary(t => t.Value, t => t.Key);
            long beginTime = WebAgent.GetTimestamps(DateTime.Now.AddDays(-7));
            long startTime = Math.Max(beginTime, request.StartTime - 5 * 60 * 1000L);
            long endTime = Math.Min(WebAgent.GetTimestamps(DateTime.Now.AddMinutes(-5)), startTime + 30 * 60 * 1000L);
            if (endTime < startTime) startTime = endTime - 5 * 60 * 1000L;

            OrderResult orderResult = new OrderResult(GameResultCode.Success)
            {
                data = new List<OrderData>(),
                startTime = startTime,
                endTime = endTime
            };

            while (true)
            {
                pageNum++;
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    { "startTime",startTime },
                    { "endTime",endTime },
                    { nameof(merchantCode),this.merchantCode },
                    { "timestamp", WebAgent.GetTimestamps() },
                    { "pageSize",100 },
                    { "pageNum",pageNum }
                };
                data.Add("signature", this.getSign(this.merchantCode, data["startTime"], data["endTime"], data["timestamp"]));

                response<orderResponse>? response = this.Post<orderResponse>(APIMethod.GetOrder, "/api/bet/queryBetList", data, out GameResultCode code, out string message);
                orderResponse? result = response?.data;

                if (result == null || code != GameResultCode.Success)
                {
                    return new OrderResult(code)
                    {
                        Message = message
                    };
                }

                int totalCount = result?.totalCount ?? 0;
                int pageSize = result?.pageSize ?? 0;
                int totalPage = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;

                foreach (order order in result?.list ?? Array.Empty<order>())
                {
                    GameAPIOrderStatus status = GameAPIOrderStatus.Wait;
                    switch (order.orderStatus)
                    {
                        case 1:
                            status = order.outcome switch
                            {
                                4 => GameAPIOrderStatus.Win,
                                5 => GameAPIOrderStatus.Win,
                                3 => GameAPIOrderStatus.Lose,
                                6 => GameAPIOrderStatus.Lose,
                                _ => GameAPIOrderStatus.Wait
                            };
                            break;
                        case 2: // 取消(人工)
                        case 4: // 风控拒单
                        case 5: // 撤单(赛事取消)
                            status = GameAPIOrderStatus.Return;
                            break;
                    }

                    IOrderData? sportData = this.getData(order, out string gameCode, out Dictionary<string, string> dictionary);

                    orderResult.data.Add(new OrderData
                    {
                        orderId = order.orderNo,
                        category = sportData?.Type ?? "Sport",
                        playerName = order.userName,
                        gameCode = order.seriesType.ToString(),
                        currency = currency.Get(order.currency),
                        createTime = order.createTime,
                        settleTime = order.settleTime ?? 0,
                        betMoney = order.orderAmount,
                        betAmount = Math.Min(order.orderAmount, Math.Abs(order.profitAmount ?? 0)),
                        money = order.profitAmount ?? 0,
                        status = status,
                        hash = order.modifyTime.ToString(),
                        rawData = order.ToJson(),
                        data = sportData
                    });
                }

                if (totalPage <= pageNum)
                {
                    return orderResult;
                }
            }
        }

        public override LoginResponse Login(LoginModel request)
        {
            if (!Currencies.ContainsKey(request.Currency)) return new LoginResponse(GameResultCode.CurrencyInvalid);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"userName",request.PlayerName },
                { "merchantCode",this.merchantCode },
                {"terminal", request.Platform == GamePlatform.Mobile ?"mobile":"pc" },
                {"timestamp",WebAgent.GetTimestamps() },
                {"currency",Currencies[request.Currency] }
            };
            data.Add("signature", this.getSign(data["merchantCode"], data["userName"], data["terminal"], data["timestamp"]));

            response<loginResponse>? response = this.Post<loginResponse>(APIMethod.Login, "/api/user/login", data, out GameResultCode code, out string message);
            if (response == null)
            {
                return new LoginResponse(code)
                {
                    Message = message
                };
            }
            loginResponse login = response.data;

            return new LoginResponse(code)
            {
                Message = message,
                Url = login.loginUrl,
                Method = LoginMethod.Redirect
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            if (!Currencies.ContainsKey(request.Currency)) return new RegisterResponse(GameResultCode.CurrencyInvalid);

            for (int index = 0; index < 5; index++)
            {
                string playerName = this.CreateUserName(request.Prefix, request.UserName, 16, index);
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"userName",playerName },
                    { nameof(merchantCode),this.merchantCode },
                    {"timestamp", WebAgent.GetTimestamps() },
                    {"currency",Currencies.Get(request.Currency) }
                };
                data.Add("signature", this.getSign(data["userName"], data[nameof(merchantCode)], data["timestamp"]));

                response<registerResponse>? response = this.Post<registerResponse>(APIMethod.Register, "/api/user/create", data, out GameResultCode
                     code, out string message);
                if (response == null)
                {
                    return new RegisterResponse(code)
                    {
                        Message = message
                    };
                }
                if (code == GameResultCode.DuplicatePlayerName) continue;

                return new RegisterResponse(code)
                {
                    PlayerName = playerName,
                    Message = message
                };
            }
            return new RegisterResponse(GameResultCode.DuplicatePlayerName);
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            string transferId = $"{WebAgent.GetTimestamps()}{WebAgent.GetRandom(100000, 999999)}";
            string money = Math.Abs(request.Money).ToString("0.00");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"userName",request.PlayerName },
                { nameof(merchantCode),this.merchantCode },
                {"transferType", request.Money >0 ?1:2 },
                {"amount", money },
                {"transferId",transferId },
                {"timestamp", WebAgent.GetTimestamps() }
            };
            data.Add("signature", this.getSign(data[nameof(merchantCode)], data["userName"], data["transferType"], data["amount"], data["transferId"], data["timestamp"]));

            response<decimal>? response = this.Post<decimal>(APIMethod.Transfer, "/api/fund/transfer", data, out GameResultCode code, out string message);
            if (response == null)
            {
                return new TransferResponse(code)
                {
                    Message = message
                };
            }

            return new TransferResponse(code)
            {
                OrderID = request.OrderID,
                TransferID = transferId,
                PlayerName = request.PlayerName,
                Money = request.Money,
                Message = message,
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
            message = result;
            try
            {
                JObject info = JObject.Parse(result);
                string code = info["code"]?.Value<string>() ?? string.Empty;
                message = info["msg"]?.Value<string>() ?? result;
                return code switch
                {
                    "0000" => GameResultCode.Success,
                    "1001" => GameResultCode.ParameterInvalid,
                    "1002" => GameResultCode.OrderNotFound,
                    "1003" => GameResultCode.SignInvalid,
                    "2001" => GameResultCode.NoMerchant,
                    "2002" => GameResultCode.NoPlayer,
                    "2003" => GameResultCode.DuplicatePlayerName,
                    "2004" => GameResultCode.PlayerLocked,
                    "2005" => GameResultCode.PlayerNameInvalid,
                    "2006" => GameResultCode.Repeated,
                    "3001" => GameResultCode.NoBalance,
                    "4002" => GameResultCode.LoginFaild,
                    "5001" => GameResultCode.SignInvalid,
                    "5002" => GameResultCode.IPInvalid,
                    "6001" => GameResultCode.MoneyInvalid,
                    "6002" => GameResultCode.TransferInvalid,
                    "6004" => GameResultCode.OrderNotFound,
                    "9001" => GameResultCode.Exception,
                    _ => GameResultCode.Error
                };
            }
            catch
            {
                return GameResultCode.Exception;
            }
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

        /// <summary>
        /// 签名加密
        /// </summary>
        private string getSign(params object[] args)
        {
            string signStr = string.Join("&", args);
            string md5 = EncryptAgent.toMD5(signStr).ToLower();
            return EncryptAgent.toMD5($"{md5}&{this.key}").ToLower();
        }


        private response<TResponse>? Post<TResponse>(APIMethod method, string path, Dictionary<string, object> data, out GameResultCode code, out string message)
        {
            GameResponse response = this.Request(new GameRequest()
            {
                Url = $"{this.gateway}{path}",
                Data = data.ToQueryString(),
                Method = method
            });
            code = this.GetResultCode(response.Content, out message);

            return JsonConvert.DeserializeObject<response<TResponse>>(response.Content);
        }

        private IOrderData? getData(order order, out string gameCode, out Dictionary<string, string> dictionary)
        {
            dictionary = new Dictionary<string, string>();
            if (order.seriesType != 1)
            {
                gameCode = $"seriesType:{order.seriesType}";
                dictionary.Add(gameCode, order.seriesValue);
                return null;
            }
            orderItem item = order.detailList.FirstOrDefault();
            gameCode = item?.sportId.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(gameCode))
            {
                dictionary.Add(gameCode, item?.sportName ?? string.Empty);
            }
            if (item == null) return null;

            dictionary.Add($"{nameof(SportData.sportId)}:{item.sportId}", item.sportName);
            dictionary.Add($"{nameof(SportData.leagueId)}:{item.tournamentId}", item.matchName);
            dictionary.Add($"{nameof(SportData.matchId)}:{item.matchId}", item.matchInfo);
            dictionary.Add($"{nameof(SportData.playType)}:{item.playId}", item.playName);
            dictionary.Add($"{nameof(SportData.betId)}:{item.playOptionsId}", item.playOptionName);

            return new SportData
            {
                sportId = item.sportId.ToString(),
                leagueId = item.tournamentId.ToString(),
                matchId = item.matchId.ToString(),
                playType = item.playId.ToString(),
                betType = item.playOptions,
                betId = item.playOptionsId.ToString(),
                //marketType = item.marketType,
                odds = item.oddsValue
            };
        }

        #endregion

        #region ========  实体类  ========

        class response<T>
        {
            public bool status { get; set; }

            public string msg { get; set; }

            public string code { get; set; }
            public T data { get; set; }

            public long serverTime { get; set; }

            public static implicit operator T(response<T> response)
            {
                if (response == null) return default;
                return response.data;
            }

        }

        class registerResponse
        {
            /// <summary>
            /// 体育服务的用户id
            /// </summary>
            public string userId { get; set; }
        }

        class loginResponse
        {
            public string userId { get; set; }

            public string domain { get; set; }

            public string loginUrl { get; set; }
        }

        class balanceResponse
        {
            public decimal balance { get; set; }

            public string userName { get; set; }
        }

        class checkTransfer
        {
            /// <summary>
            /// 交易id
            /// </summary>
            public string transferId { get; set; }

            /// <summary>
            /// 交易类型 1：加款，2：扣款
            /// </summary>
            public int transferType { get; set; }

            /// <summary>
            /// 转账成功与否(0:失败，1:成功)
            /// </summary>
            public int status { get; set; }

            /// <summary>
            /// 转账金额
            /// </summary>
            public decimal amount { get; set; }
        }

        class orderResponse
        {
            public int pageSize { get; set; }

            public int totalCount { get; set; }

            public order[]? list { get; set; }
        }

        class order
        {
            /// <summary>
            /// 商户编码
            /// </summary>
            public string merchantCode { get; set; }

            /// <summary>
            /// 用户名称
            /// </summary>
            public string userName { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 订单状态    
            /// 0	待处理 1	已结算 2	取消(人工)  3	待确认 4	风控拒单    5	撤单(赛事取消)
            /// </summary>
            public int? orderStatus { get; set; }

            /// <summary>
            /// 该订单下的注单数(单关为1，串关为n)
            /// </summary>
            public int betCount { get; set; }

            /// <summary>
            /// 串关类型
            /// </summary>
            public int seriesType { get; set; }

            /// <summary>
            /// 串关值
            /// </summary>
            public string seriesValue { get; set; }

            /// <summary>
            /// 订单id
            /// </summary>
            public string orderNo { get; set; }

            /// <summary>
            /// 投注时间(13位时间戳)
            /// </summary>
            public long createTime { get; set; }

            /// <summary>
            /// 订单更新时间
            /// </summary>
            public long modifyTime { get; set; }

            /// <summary>
            /// 订单实际投注金额
            /// </summary>
            public decimal orderAmount { get; set; }

            /// <summary>
            /// 订单结算结果 0：无结果，2：走水，3：输，4：赢，5：赢一半，6：输一半
            /// </summary>
            public int? outcome { get; set; }

            /// <summary>
            /// 结算金额
            /// </summary>
            public decimal? settleAmount { get; set; }

            public decimal? profitAmount { get; set; }

            /// <summary>
            /// 盈利金额
            /// </summary>
            public decimal? preBetAmount { get; set; }

            /// <summary>
            /// 最大中奖金额
            /// </summary>
            public decimal? maxWinAmount { get; set; }

            /// <summary>
            /// 结算时间
            /// </summary>
            public long? settleTime { get; set; }

            /// <summary>
            /// 结算次数
            /// </summary>
            public int? settleTimes { get; set; }

            /// <summary>
            /// 汇率
            /// </summary>
            public double? exchangeRate { get; set; }

            /// <summary>
            /// 设备类型 1：H5，2：PC，3：Android，4：IOS
            /// </summary>
            public int deviceType { get; set; }

            /// <summary>
            /// IP地址
            /// </summary>
            public string ip { get; set; }

            /// <summary>
            /// 移动设备标识
            /// </summary>
            public string deviceImei { get; set; }

            /// <summary>
            /// vip等级
            /// </summary>
            public int vipLevel { get; set; }

            /// <summary>
            /// 订单明细
            /// </summary>
            public orderItem[] detailList { get; set; }
        }

        /// <summary>
        /// 订单的明细
        /// </summary>
        class orderItem
        {
            /// <summary>
            /// 注单id
            /// </summary>
            public long betNo { get; set; }

            /// <summary>
            /// 联赛id
            /// </summary>
            public long tournamentId { get; set; }

            /// <summary>
            /// 投注项id
            /// </summary>
            public long playOptionsId { get; set; }

            /// <summary>
            /// 赛事id
            /// </summary>
            public long matchId { get; set; }

            /// <summary>
            /// 比赛开始时间
            /// </summary>
            public long beginTime { get; set; }

            /// <summary>
            /// 注单金额
            /// </summary>
            public decimal betAmount { get; set; }

            /// <summary>
            /// 联赛名称
            /// </summary>
            public string matchName { get; set; }

            /// <summary>
            /// 比赛对阵
            /// </summary>
            public string matchInfo { get; set; }

            /// <summary>
            /// 投注类型 1：早盘赛事，2：滚球盘赛事，3：冠军盘赛事。
            /// 若为其他值，则为冗馀字段，请忽略
            /// </summary>
            public int matchType { get; set; }

            /// <summary>
            /// 盘口类型
            /// </summary>
            public string marketType { get; set; }

            /// <summary>
            /// 赛种id
            /// </summary>
            public int sportId { get; set; }

            /// <summary>
            /// 游戏名称
            /// </summary>
            public string sportName { get; set; }

            /// <summary>
            /// 投注项名称
            /// </summary>
            public string playOptionName { get; set; }

            /// <summary>
            /// 玩法名称
            /// </summary>
            public string playName { get; set; }

            /// <summary>
            /// 盘口值
            /// </summary>
            public string marketValue { get; set; }

            /// <summary>
            /// 让分值（冗余字段）
            /// </summary>
            public string handicap { get; set; }

            /// <summary>
            /// 赔率
            /// </summary>
            public decimal oddsValue { get; set; }

            /// <summary>
            /// 原始赔率
            /// </summary>
            public decimal oddFinally { get; set; }

            /// <summary>
            /// 注单结算结果 0：无结果，2：走水，3：输，4：赢，5：赢一半，6：输一半
            /// </summary>
            public int betResult { get; set; }

            /// <summary>
            /// 基准分
            /// </summary>
            public string scoreBenchmark { get; set; }

            /// <summary>
            /// 结算比分(该字段谨慎使用，由于数据商未下发此数据，该字段为我方根据赛事事件处理后获取)
            /// </summary>
            public string settleScore { get; set; }

            /// <summary>
            /// 投注项，如主客队
            /// </summary>
            public string playOptions { get; set; }

            /// <summary>
            /// 玩法id
            /// </summary>
            public int playId { get; set; }

            public int? cancelType { get; set; }
        }

        #endregion
    }
}
