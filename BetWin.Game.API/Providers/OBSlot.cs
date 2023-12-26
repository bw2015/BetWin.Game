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
using System.Xml.Linq;

namespace BetWin.Game.API.Providers
{
    [Description("OB电子")]
    internal class OBSlot : GameBase
    {
        [Description("网关")]
        public string gateway { get; set; }

        [Description("密钥")]
        public string Key { get; set; }

        [Description("IV")]
        public string IV { get; set; }

        [Description("商户编号")]
        public string Agent { get; set; }

        [Description("操作IP")]
        public string MemberIP { get; set; } = "0.0.0.0";

        [Description("订单网关")]
        public string Order { get; set; }

        [Description("线路币种")]
        public GameCurrency Currency { get; set; } = GameCurrency.CNY;



        public OBSlot(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>
        {

        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>
        {

        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"memberId",request.PlayerName },
                {"memberPwd",request.Password ?? this.defaultPasssword },
                {"memberIp",this.MemberIP }
            };

            response<balanceResponse>? result = this.Post<balanceResponse>(APIMethod.Balance, "queryBalance", data, out GameResultCode code, out string message);

            return new BalanceResponse(code)
            {
                Message = message,
                Currency = request.Currency,
                Balance = (result?.data?.balance ?? 0) / 100M
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"orderId",request.OrderID }
            };

            response<checkTransfer>? result = this.Post<checkTransfer>(APIMethod.CheckTransfer, "queryOrderStatus", data, out GameResultCode code, out string message);

            return new CheckTransferResponse(code)
            {
                Message = message,
                Currency = request.Currency,
                Money = result?.data?.money / 100M,
                TransferID = result?.data?.orderId ?? request.OrderID,
                Status = code != GameResultCode.Success ? GameAPITransferStatus.Unknow : (result?.data?.status switch
                {
                    0 => GameAPITransferStatus.Success,
                    _ => GameAPITransferStatus.Faild
                })
            };

        }

        private long beginTime => WebAgent.GetTimestamps(DateTime.Now.AddDays(-7));

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            // 重复查询1分钟的数据，避免接口延迟导致的掉单
            DateTime startTime = WebAgent.GetTimestamps(Math.Max(beginTime, request.StartTime));

            if (startTime.TimeOfDay.TotalSeconds > 60)
            {
                startTime = startTime.AddMinutes(-1);
            }

            DateTime endTime = startTime.AddMinutes(30);

            if (endTime > DateTime.Now) endTime = DateTime.Now;
            if (startTime > endTime.AddMinutes(-5)) startTime = endTime.AddMinutes(-5);

            bool newDay = false;
            //# 如果不在一天
            if (startTime.Date != endTime.Date)
            {
                newDay = true;
                endTime = startTime.Date.AddDays(1).AddSeconds(-1);
            }

            int pageNum = 1;
            int pageSize = 10000;
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"beginTime", WebAgent.GetTimestamp(startTime) },
                {"endTime", WebAgent.GetTimestamp(endTime) },
                {"pageNum",pageNum },
                {"pageSize",pageSize }
            };

            if (newDay) endTime = endTime.AddSeconds(1);
            OrderResult orderResult = new OrderResult(GameResultCode.Success)
            {
                startTime = WebAgent.GetTimestamps(startTime),
                endTime = WebAgent.GetTimestamps(endTime),
                data = new List<OrderData>()
            };

            while (true)
            {
                data[nameof(pageNum)] = pageNum;
                var result = this.Post<orders>(APIMethod.GetOrder, "queryGameOrders", data, out GameResultCode code, out string message);
                if (code != GameResultCode.Success)
                {
                    return new OrderResult(code)
                    {
                        Message = message
                    };
                }

                foreach (var order in result?.data?.list ?? Array.Empty<order>())
                {
                    var status = GameAPIOrderStatus.Wait;
                    if (order.mw > 0)
                    {
                        status = GameAPIOrderStatus.Win;
                    }
                    else if (order.mw < 0)
                    {
                        status = GameAPIOrderStatus.Lose;
                    }
                    else
                    {
                        status = GameAPIOrderStatus.Return;
                    }

                    orderResult.data.Add(new OrderData()
                    {
                        orderId = order.bi,
                        playerName = order.mmi,
                        clientIp = "0.0.0.0",
                        gameCode = order.gi.ToString(),
                        category = GameCategory.Slot.ToString(),
                        currency = this.Currency,
                        createTime = order.st * 1000L,
                        settleTime = order.et * 1000L,
                        betMoney = order.tb / 100M,
                        betAmount = order.bc / 100M,
                        money = order.mw / 100M,
                        status = status,
                        hash = $"{order.bi}:{order.mw}",
                        rawData = order.ToJson()
                    });
                }
                pageNum++;
                if (pageNum * pageSize >= result?.data.total) break;
            }

            return orderResult;
        }

        public override LoginResponse Login(LoginModel request)
        {
            int.TryParse(request.StartCode, out int gameId);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"memberId",request.PlayerName},
                {"memberName",request.PlayerName},
                {"memberPwd", request.PlayerPassword },
                {"deviceType", request.Platform switch{
                     GamePlatform.PC => 0,
                     GamePlatform.Mobile => 1,
                     _=>0
                    }
                },
                {"memberIp",request.clientIp },
                {"gameId",gameId }
            };

            response<string>? response = this.Post<string>(APIMethod.Login, "launchGameById", data, out GameResultCode code, out string message);

            return new LoginResponse(code)
            {
                Message = message,
                Method = LoginMethod.Redirect,
                Url = response?.data
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            string playerName = this.CreateUserName(request.Prefix, request.UserName);

            return new RegisterResponse(GameResultCode.Success)
            {
                PlayerName = playerName,
                Password = this.defaultPasssword
            };
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            string action = request.Money > 0 ? "transferIn" : "transferOut";
            string orderId = $"{request.PlayerName}:{WebAgent.GetTimestamps()}";

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"memberId", request.PlayerName },
                {"money",(int)Math.Abs(request.Money*100) },
                {"orderId", orderId },
                {"memberName",request.PlayerName },
                {"memberPwd",request.Password },
                {"deviceType",0 },
                {"memberIp", this.MemberIP},
            };

            var result = this.Post<transfer>(APIMethod.Transfer, action, data, out GameResultCode code, out string message);

            return new TransferResponse(code)
            {
                Message = message,
                Balance = (result?.data?.balance ?? 0) / 100M,
                Currency = request.Currency,
                Money = (action == "transferIn" ? result?.data?.transferIn : result?.data?.transferOut) / 100M ?? 0M,
                OrderID = request.OrderID,
                PlayerName = request.PlayerName,
                TransferID = orderId,
                Status = code switch
                {
                    GameResultCode.Exception => GameAPITransferStatus.Unknow,
                    GameResultCode.Success => GameAPITransferStatus.Success,
                    _ => GameAPITransferStatus.Faild
                }
            };

        }

        public override string QueryData()
        {
            return this.RequestAPI(new GameRequest()
            {
                Url = $"{this.gateway}/gameConfig?{this.getParams()}"
            });
        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            response? res = result.ToJson<response>();
            message = res?.msg ?? result;
            if (res?.code == null) return GameResultCode.Exception;
            return res.code switch
            {
                1000 => GameResultCode.Success,
                1001 => GameResultCode.ParameterInvalid,
                1002 => GameResultCode.ParameterInvalid,
                2005 => GameResultCode.ParameterInvalid,
                1003 => GameResultCode.Timeout,
                2001 => GameResultCode.PlayerLocked,
                2002 => GameResultCode.PlayerLocked,
                2008 => GameResultCode.PlayerLocked,
                2009 => GameResultCode.PlayerLocked,
                2007 => GameResultCode.NoMerchant,
                2010 => GameResultCode.SignInvalid,
                3004 => GameResultCode.Timeout,
                3005 => GameResultCode.TransferDuplicate,
                3010 => GameResultCode.TransferDuplicate,
                3007 => GameResultCode.MoneyInvalid,
                3017 => GameResultCode.MoneyInvalid,
                3008 => GameResultCode.TransferInvalid,
                3011 => GameResultCode.NoPlayer,
                5001 => GameResultCode.NoPlayer,
                5002 => GameResultCode.NoPlayer,
                3014 => GameResultCode.NoBalance,
                4002 => GameResultCode.TransferStatus,
                7001 => GameResultCode.SystemBuzy,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            string body = string.IsNullOrEmpty(request.Data) ? string.Empty : EncryptAgent.AesEncrypt(request.Data, this.Key, this.IV, CipherMode.CBC);
            using (HttpClient client = new HttpClient())
            {
                return client.Post(request.Url, body, new Dictionary<string, string>()
                {
                    {"Content-Type","text/plain" }
                });
            }
        }

        private response<T>? Post<T>(APIMethod method, string name, Dictionary<string, object>? data, out GameResultCode code, out string message)
        {
            string host = this.gateway;
            if (method == APIMethod.GetOrder) host = this.Order;
            GameResponse result = this.Request(new GameRequest()
            {
                Method = method,
                Data = data?.ToJson() ?? string.Empty,
                Url = $"{host}/{name}?{this.getParams()}"
            });

            code = result.Code;
            message = result.Message;

            return result.Content.ToJson<response<T>>();
        }

        public override int CollectDelay => 10 * 1000;

        #region ========  工具方法  ========

        private string getSign(out long timestamp)
        {
            timestamp = WebAgent.GetTimestamp(DateTime.Now);
            string md5 = $"{this.Agent}{timestamp}{this.Key}".toMD5().ToLower();
            string randStr = Guid.NewGuid().ToString("N")[..8];
            string sign = $"{randStr[..2]}{md5[..9]}{randStr.Substring(2, 2)}{md5.Substring(9, 8)}{randStr.Substring(4, 2)}{md5[17..]}{randStr[6..]}";
            return sign;
        }

        private string getParams()
        {
            string sign = this.getSign(out long timestamp);
            string randStr = Guid.NewGuid().ToString("N")[..10];
            return $"agent={this.Agent}&timestamp={timestamp}&randno={randStr}&sign={sign}";
        }

        #endregion

        #region ========  实体类  ========

        class response
        {
            public int code { get; set; }

            public string msg { get; set; }
        }

        class response<T> : response
        {
            public T data { get; set; }
        }

        class balanceResponse
        {
            public int balance { get; set; }
        }

        class transfer
        {
            /// <summary>
            /// 转入金额
            /// </summary>
            public int transferIn { get; set; }

            public int transferOut { get; set; }

            /// <summary>
            /// 操作之后的余额
            /// </summary>
            public int balance { get; set; }
        }

        class checkTransfer
        {
            /// <summary>
            /// 0:成功，1:失败，2:无此订单
            /// </summary>
            public int status { get; set; }

            public int money { get; set; }

            public string orderId { get; set; }

            /// <summary>
            /// 0：上分 1：下分
            /// </summary>
            public int type { get; set; }
        }

        class orders
        {
            public order[] list { get; set; }
            public int pageNum { get; set; }

            public int pageSize { get; set; }

            public int total { get; set; }
        }

        class order
        {
            /// <summary>
            /// 注单 id
            /// </summary>
            public string bi { get; set; }

            /// <summary>
            /// 商户 id
            /// </summary>
            public int mi { get; set; }

            /// <summary>
            /// 玩家账号(用户名)
            /// </summary>
            public string mmi { get; set; }

            /// <summary>
            /// 投注时间
            /// </summary>
            public long st { get; set; }

            /// <summary>
            /// 结算时间
            /// </summary>
            public long et { get; set; }

            /// <summary>
            /// 游戏桌号
            /// </summary>
            public int gd { get; set; }

            /// <summary>
            /// 游戏 id
            /// </summary>
            public int gi { get; set; }

            /// <summary>
            /// 游戏名称
            /// </summary>
            public string gn { get; set; }

            /// <summary>
            /// 房间类型(1：初级，2：中级...)
            /// </summary>
            public int gt { get; set; }

            /// <summary>
            /// 游戏房间
            /// </summary>
            public string gr { get; set; }

            /// <summary>
            /// 输赢金额
            /// </summary>
            public int mw { get; set; }

            /// <summary>
            /// 抽水金额
            /// </summary>
            public int mp { get; set; }

            /// <summary>
            /// 有效投注
            /// </summary>
            public int bc { get; set; }

            /// <summary>
            /// 终端设备类型（0:web,1:h5,2：ios,3:android）
            /// </summary>
            public int dt { get; set; }

            /// <summary>
            /// 总投注金额
            /// </summary>
            public int tb { get; set; }

            /// <summary>
            /// 局号：用于游戏内、后台展示和查询
            /// </summary>
            public string cn { get; set; }

            /// <summary>
            /// 游戏分类标记（0:游戏类，100:活动类）
            /// </summary>
            public int gf { get; set; }
        }

        #endregion
    }
}
