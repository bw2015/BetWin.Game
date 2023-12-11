using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.API.Providers
{
    [Description("开元棋牌")]
    internal class KYChess : GameBase
    {
        [Description("网关")]
        public string gateway { get; set; }

        [Description("商户号")]
        public string agent { get; set; }

        [Description("DES密钥")]
        public string DESKey { get; set; }

        [Description("MD5密钥")]
        public string MD5Key { get; set; }

        [Description("线路币种")]
        public GameCurrency Currency { get; set; } = GameCurrency.CNY;

        /// <summary>
        /// 订单日志采集的服务器
        /// </summary>
        [Description("订单日志")]
        public string logServer { get; set; }

        public KYChess(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>() {
            {GameLanguage.ENG,"en_us" },
            {GameLanguage.VI,"vi_vn" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>() {
            { GameCurrency.CNY,"RMB" },
            { GameCurrency.USD,"USD" },
            { GameCurrency.TWD,"TWD" },
            { GameCurrency.MYR,"MYR" },
            { GameCurrency.VND,"VND" },
            { GameCurrency.KVND,"VND2" },
            { GameCurrency.THB,"THB" },
            { GameCurrency.IDR,"IDR" },
            { GameCurrency.JPY,"JPY" },
            { GameCurrency.KRW,"KRW" },
            { GameCurrency.INR,"INR" },
            { GameCurrency.EUR,"EUR" },
            { GameCurrency.GBP,"GBP" },
            { GameCurrency.MMK,"MMK" },
            { GameCurrency.KMMK,"MMK2" },
            { GameCurrency.BRL,"BRL" }
        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "s",1 },
                {"account",request.PlayerName}
            };
            response<balance>? balance = this.Post<balance>(APIMethod.Balance, data, out GameResultCode code, out string message);
            return new BalanceResponse(code)
            {
                Balance = balance?.d?.money ?? 0,
                Message = message,
                Currency = request.Currency
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"s",4 },
                {"orderid",request.OrderID }
            };
            response<transferCheck>? transferCheck = this.Post<transferCheck>(APIMethod.CheckTransfer, data, out GameResultCode code, out string message);
            return new CheckTransferResponse(code)
            {
                Money = transferCheck?.d?.money ?? 0M,
                TransferID = request.OrderID,
                Status = transferCheck?.d?.status switch
                {
                    -1 => GameAPITransferStatus.Faild,
                    0 => GameAPITransferStatus.Success,
                    2 => GameAPITransferStatus.Faild,
                    _ => GameAPITransferStatus.Unknow
                }
            };
        }

        private long beginTime = WebAgent.GetTimestamps(DateTime.Now.AddDays(-7));

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            long startTime = Math.Max(this.beginTime, request.StartTime);
            long endTime = Math.Min(WebAgent.GetTimestamps(), startTime + 30 * 60 * 1000);
            startTime = Math.Min(startTime, endTime - 5 * 60 * 1000);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"s",6 },
                {"startTime",startTime },
                {"endTime",endTime }
            };

            response<orderResponse>? response = this.Post<orderResponse>(APIMethod.GetOrder, data, out GameResultCode code, out string message);
            if (code != GameResultCode.Success) return new OrderResult(code);

            OrderResult result = new OrderResult(code)
            {
                data = new List<OrderData>(),
                startTime = startTime,
                endTime = endTime
            };

            for (int index = 0; index < response?.d.count; index++)
            {
                orderItemData item = response?.d.list.GetData(index) ?? default;
                string rawData = item.ToJson();
                decimal profit = item.Profit;
                result.data.Add(new OrderData
                {
                    category = "Chess",
                    orderId = item.GameID,
                    playerName = item.Accounts[(this.agent.Length + 1)..],
                    createTime = WebAgent.GetTimestamps(item.GameStartTime, TimeSpan.FromHours(8)),
                    currency = this.Currency,
                    hash = rawData.toMD5().ToLower(),
                    gameCode = item.KindID.ToString(),
                    betMoney = item.AllBet,
                    betAmount = item.CellScore,
                    money = profit,
                    rawData = rawData,
                    settleTime = WebAgent.GetTimestamps(item.GameEndTime, TimeSpan.FromHours(8)),
                    status = profit == 0 ? GameAPIOrderStatus.Return : (profit > 0 ? GameAPIOrderStatus.Win : GameAPIOrderStatus.Lose)
                });
            }

            return result;
        }

        public override LoginResponse Login(LoginModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"s",0 },
                {"account",request.PlayerName },
                {"money",0 },
                {"orderid",$"{this.agent}{DateTime.Now:yyyyMMddHHmmssfff}{request.PlayerName}" },
                {"ip",request.clientIp },
                {"lineCode",request.SiteID },
                {"KindID",request.StartCode }
            };
            response<login>? login = this.Post<login>(APIMethod.Login, data, out GameResultCode code, out string message);
            return new LoginResponse(code)
            {
                Message = message,
                Method = LoginMethod.Redirect,
                Url = login?.d?.url
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"s",8 },
                {"account",request.PlayerName }
            };

            response<responseBase>? response = this.Post<responseBase>(APIMethod.Logout, data, out GameResultCode code, out string message);
            return new LogoutResponse(code)
            {
                PlayerName = request.PlayerName,
                Message = message
            };
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            return new RegisterResponse(GameResultCode.Success)
            {
                PlayerName = this.CreateUserName(request.Prefix, request.UserName, 20),
                Password = string.Empty,
                Message = string.Empty
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            string transferId = $"{this.agent}{DateTime.Now:yyyyMMddHHmmssfff}{request.PlayerName}";
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"s", request.Money>0 ? 2: 3 },
                {"account", request.PlayerName },
                {"money", Math.Abs(request.Money) },
                {"orderid",transferId }
            };
            response<transfer>? transfer = this.Post<transfer>(APIMethod.Transfer, data, out GameResultCode code, out string message);
            return new TransferResponse(code)
            {
                Money = request.Money,
                OrderID = request.OrderID,
                PlayerName = request.PlayerName,
                TransferID = transferId,
                Status = code switch
                {
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    GameResultCode.NoBalance => GameAPITransferStatus.Faild,
                    GameResultCode.MoneyInvalid => GameAPITransferStatus.Faild,
                    _ => GameAPITransferStatus.Unknow
                }
            };
        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            responseBase? response = result.ToJson<responseBase>();
            message = string.Empty;
            if (response == null) return GameResultCode.Exception;
            return response?.code switch
            {
                -1 => GameResultCode.TransferFalid,
                0 => GameResultCode.Success,

                //数据不存在（当前没有注单）
                16 => GameResultCode.Success,

                1 => GameResultCode.LoginFaild,
                2 => GameResultCode.GameCodeInvalid,
                3 => GameResultCode.Timeout,
                5 => GameResultCode.IPInvalid,
                6 => GameResultCode.SignInvalid,
                8 => GameResultCode.ConfigInvalid,
                15 => GameResultCode.SignInvalid,
                20 => GameResultCode.PlayerLocked,
                22 => GameResultCode.SignInvalid,
                24 => GameResultCode.ParameterInvalid,
                27 => GameResultCode.Error,
                28 => GameResultCode.IPInvalid,
                30 => GameResultCode.Error,
                31 => GameResultCode.MoneyInvalid,
                32 => GameResultCode.Error,
                33 => GameResultCode.Error,
                34 => GameResultCode.TransferDuplicate,
                35 => GameResultCode.NoPlayer,
                36 => GameResultCode.GameCodeInvalid,
                37 => GameResultCode.SystemBuzy,
                38 => GameResultCode.NoBalance,
                39 => GameResultCode.SystemBuzy,
                40 => GameResultCode.MoneyInvalid,
                41 => GameResultCode.ParameterInvalid,
                42 => GameResultCode.SiteLock,
                43 => GameResultCode.SystemBuzy,
                44 => GameResultCode.TransferDuplicate,
                45 => GameResultCode.MoneyInvalid,
                1002 => GameResultCode.SiteNoBalance,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                return client.Get(request.Url, new Dictionary<string, string>()
                {
                    {"Content-Type","application/json" }
                });
            }
        }

        private response<T>? Post<T>(APIMethod method, Dictionary<string, object> data, out GameResultCode code, out string message) where T : responseBase
        {

            long timestamp = WebAgent.GetTimestamps();
            string param = EncryptAgent.AesEncrypt(data.ToQueryString(), this.DESKey);
            string key = $"{this.agent}{timestamp}{this.MD5Key}".toMD5().ToLower();

            Dictionary<string, object> _params = new Dictionary<string, object>
            {
                {"agent",this.agent },
                {"timestamp",timestamp },
                {"param",param },
                {"key",key }
            };
            data.Add("param", param);
            data.Add("key", key);
            data.Add("timestamp", timestamp);

            string url = $"{this.gateway}?{_params.ToQueryString(true)}";
            switch (method)
            {
                case APIMethod.GetOrder:
                    url = $"{this.logServer}?{_params.ToQueryString(true)}";
                    break;
            }
            var result = this.Request(new GameRequest()
            {
                Url = url,
                Data = data.ToJson(),
                Method = method
            });
            code = result.Code;
            message = result.Message;
            return result.Content.ToJson<response<T>>();
        }

        #region ========  实体类  ========

        protected class response<T> where T : responseBase
        {
            public string m { get; set; }

            public int s { get; set; }

            public T d { get; set; }

            public static implicit operator T(response<T> response)
            {
                return response.d;
            }
        }

        protected class responseBase
        {
            public int code { get; set; }
        }

        protected class login : responseBase
        {
            /// <summary>
            /// 登录地址
            /// </summary>
            public string url { get; set; }
        }

        protected class balance : responseBase
        {
            public decimal money { get; set; }
        }

        protected class transfer : responseBase
        {
            /// <summary>
            /// 转账之后的余额
            /// </summary>
            public decimal money { get; set; }
        }

        /// <summary>
        /// 转账订单查询
        /// </summary>
        protected class transferCheck : responseBase
        {
            /// <summary>
            /// 状态码 Status code（-1：不存在 inexistence、0：成功 success、2: 失败 failure、3:正在处理中 processing）
            /// </summary>
            public int status { get; set; }

            public decimal money { get; set; }
        }

        /// <summary>
        /// 订单查询返回
        /// </summary>
        protected class orderResponse : responseBase
        {
            public long start { get; set; }

            public long end { get; set; }

            public int count { get; set; }

            public orderItem list { get; set; }
        }

        /// <summary>
        /// 订单内容
        /// </summary>
        public class orderItem
        {
            //GameID":["50-1660014906-861923379-6"],"Accounts":["10001_test01"],
            //"ServerID":[3703],"KindID":[620],"TableID":[74060002],"ChairID":[6],"UserCount":[5],"CellScore":[1160],"
            //AllBet":[2440],"Profit":[-1160],"Revenue":[0],"GameStartTime":["2022-08-09 11:15:06"],
            //"GameEndTime":["2022-08-09 11:15:41","2022-08-09 11:17:08"],
            //"CardValue":["0000222a0000353d0000270700000637321c192b182d11"],
            //"ChannelID":[10001],"LineCode":["10001_LC_123"]}

            /// <summary>
            /// 游戏局号列表 Game number list
            /// </summary>
            public string[] GameID { get; set; }

            /// <summary>
            /// 玩家帐号列表 player account list
            /// </summary>
            public string[] Accounts { get; set; }

            /// <summary>
            /// 房间 ID 列表 Room ID list
            /// </summary>
            public int[] ServerID { get; set; }

            /// <summary>
            /// 游戏 ID 列表(游戏见附录) Game ID list (refer to appendix)
            /// </summary>
            public int[] KindID { get; set; }

            /// <summary>
            /// 桌子号列表 Table number list
            /// </summary>
            public int[] TableID { get; set; }

            /// <summary>
            /// 椅子号列表 Chair number
            /// </summary>
            public int[] ChairID { get; set; }

            /// <summary>
            /// 玩家数量列表 Number of players list
            /// </summary>
            public int[] UserCount { get; set; }

            /// <summary>
            /// 手牌公共牌（读取规则见附录）Hand Community Cards（the reading rules refer to appendix）
            /// </summary>
            public string[] CardValue { get; set; }

            /// <summary>
            /// 有效下注 Effective bet
            /// </summary>
            public decimal[] CellScore { get; set; }

            /// <summary>
            /// 总下注列表 Total bet list
            /// </summary>
            public decimal[] AllBet { get; set; }

            /// <summary>
            /// 盈利列表 Profit list
            /// </summary>
            public decimal[] Profit { get; set; }

            /// <summary>
            /// 抽水列表 Percentage list
            /// </summary>
            public decimal[] Revenue { get; set; }

            /// <summary>
            /// 游戏开始时间列表 Game start Time list
            /// 北京时间
            /// </summary>
            public DateTime[] GameStartTime { get; set; }

            /// <summary>
            /// 游戏结束时间列表 Game end Time list
            /// 北京时间
            /// </summary>
            public DateTime[] GameEndTime { get; set; }

            /// <summary>
            /// 渠道 ID 列表 Channel ID list
            /// </summary>
            public int[] ChannelID { get; set; }

            /// <summary>
            /// 游戏结果对应玩家所属站点 The result of the game corresponds to the players site.
            /// </summary>
            public string[] LineCode { get; set; }

            public orderItemData GetData(int index)
            {
                return new orderItemData
                {
                    GameID = this.GameID[index],
                    Accounts = this.Accounts[index],
                    ServerID = this.ServerID[index],
                    KindID = this.KindID[index],
                    TableID = this.TableID[index],
                    ChairID = this.ChairID[index],
                    UserCount = this.UserCount[index],
                    CardValue = this.CardValue[index],
                    CellScore = this.CellScore[index],
                    AllBet = this.AllBet[index],
                    Profit = this.Profit[index],
                    Revenue = this.Revenue[index],
                    GameStartTime = this.GameStartTime[index],
                    GameEndTime = this.GameEndTime[index],
                    ChannelID = this.ChannelID[index],
                    LineCode = this.LineCode[index]
                };
            }
        }

        public struct orderItemData
        {
            /// <summary>
            /// 游戏局号列表 Game number list
            /// </summary>
            public string GameID { get; set; }

            /// <summary>
            /// 玩家帐号列表 player account list
            /// </summary>
            public string Accounts { get; set; }

            /// <summary>
            /// 房间 ID 列表 Room ID list
            /// </summary>
            public int ServerID { get; set; }

            /// <summary>
            /// 游戏 ID 列表(游戏见附录) Game ID list (refer to appendix)
            /// </summary>
            public int KindID { get; set; }

            /// <summary>
            /// 桌子号列表 Table number list
            /// </summary>
            public int TableID { get; set; }

            /// <summary>
            /// 椅子号列表 Chair number
            /// </summary>
            public int ChairID { get; set; }

            /// <summary>
            /// 玩家数量列表 Number of players list
            /// </summary>
            public int UserCount { get; set; }

            /// <summary>
            /// 手牌公共牌（读取规则见附录）Hand Community Cards（the reading rules refer to appendix）
            /// </summary>
            public string CardValue { get; set; }

            /// <summary>
            /// 有效下注 Effective bet
            /// </summary>
            public decimal CellScore { get; set; }

            /// <summary>
            /// 总下注列表 Total bet list
            /// </summary>
            public decimal AllBet { get; set; }

            /// <summary>
            /// 盈利列表 Profit list
            /// </summary>
            public decimal Profit { get; set; }

            /// <summary>
            /// 抽水列表 Percentage list
            /// </summary>
            public decimal Revenue { get; set; }

            /// <summary>
            /// 游戏开始时间列表 Game start Time list
            /// 北京时间
            /// </summary>
            public DateTime GameStartTime { get; set; }

            /// <summary>
            /// 游戏结束时间列表 Game end Time list
            /// 北京时间
            /// </summary>
            public DateTime GameEndTime { get; set; }

            /// <summary>
            /// 渠道 ID 列表 Channel ID list
            /// </summary>
            public int ChannelID { get; set; }

            /// <summary>
            /// 游戏结果对应玩家所属站点 The result of the game corresponds to the players site.
            /// </summary>
            public string LineCode { get; set; }
        }

        #endregion
    }
}
