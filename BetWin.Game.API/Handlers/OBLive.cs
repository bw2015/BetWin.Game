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

namespace BetWin.Game.API.Handlers
{
    [Description("OB真人")]
    internal class OBLive : GameBase
    {
        [Description("网关")]
        public string gateway { get; set; }

        [Description("商户号")]
        public string merchantCode { get; set; }

        [Description("aesKey")]
        public string aesKey { get; set; }

        [Description("md5Key")]
        public string md5Key { get; set; }

        [Description("商户前缀")]
        public string prefix { get; set; }

        [Description("数据接口")]
        public string LogServer { get; set; } = "https://api-data.obothapi.com";

        public OBLive(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>()
        {
            { GameLanguage.CHN,"1" },
            { GameLanguage.THN,"2" },
            { GameLanguage.ENG,"3" },
            { GameLanguage.JA,"4" },
            { GameLanguage.KO,"5" },
            { GameLanguage.VI,"6" },
            { GameLanguage.IND,"8" },
            { GameLanguage.AR,"9" },
            { GameLanguage.DE,"10" },
            { GameLanguage.ES,"11" },
            { GameLanguage.FR,"12" },
            { GameLanguage.RU,"13" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>()
        {
            { GameCurrency.CNY,"1" },
            { GameCurrency.KRW,"2" },
            { GameCurrency.MYR,"3" },
            { GameCurrency.USD,"4" },
            { GameCurrency.JPY,"5" },
            { GameCurrency.THB,"6" },
            { GameCurrency.BTC,"7" },
            { GameCurrency.VND,"8" },
            { GameCurrency.EUR,"9" },
            { GameCurrency.HKD,"10" },
            { GameCurrency.AUD,"11" },
            { GameCurrency.TWD,"12" },
            { GameCurrency.IDR,"13" },
            { GameCurrency.INR,"14" },
            { GameCurrency.BND,"15" }
        };

        public override BalanceResponse Balance(BalanceModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "loginName",request.PlayerName }
            };
            balanceResponse? balance = this.Post<balanceResponse>(APIMethod.Balance, "/api/merchant/balance/v1", data, out GameResultCode code, out string message);
            return new BalanceResponse(code)
            {
                Balance = balance?.balance ?? 0,
                Currency = request.Currency,
                Message = message
            };
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"transferNo", request.OrderID },
                {"loginName", request.PlayerName }
            };
            checkTransfer? checkTransfer = this.Post<checkTransfer>(APIMethod.CheckTransfer, "/api/merchant/transfer/v1", data, out GameResultCode code, out string message);

            GameAPITransferStatus status = GameAPITransferStatus.Unknow;
            if (code == GameResultCode.Success)
            {
                status = checkTransfer?.transferStatus switch
                {
                    0 => GameAPITransferStatus.Success,
                    1 => GameAPITransferStatus.Faild,
                    _ => GameAPITransferStatus.Unknow
                };
            }
            else if (code != GameResultCode.Exception)
            {
                status = GameAPITransferStatus.Faild;
            }

            return new CheckTransferResponse(code)
            {
                Message = message,
                TransferID = checkTransfer?.tradeNo ?? request.OrderID,
                Money = checkTransfer?.amount ?? 0,
                Status = status
            };
        }

        private long beginTime => WebAgent.GetTimestamps(DateTime.Now.AddDays(-7));
        private TimeSpan duration => TimeSpan.FromMinutes(30);

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            DateTime startTime = WebAgent.GetTimestamps(Math.Max(beginTime, request.StartTime));
            DateTime endTime = startTime.Add(duration);

            if (endTime > DateTime.Now.AddSeconds(-40)) endTime = DateTime.Now.AddSeconds(-40);
            if (startTime > endTime) startTime = endTime.AddMinutes(-5);

            Dictionary<string, GameCurrency> currency = this.Currencies.ToDictionary(t => t.Value, t => t.Key);

            int totalPage = 1,
               pageIndex = 1;
            OrderResult result = new OrderResult(GameResultCode.Success)
            {
                startTime = WebAgent.GetTimestamps(startTime),
                endTime = WebAgent.GetTimestamps(endTime),
                data = new List<OrderData>()
            };

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss")  },
                {"endTime",endTime.ToString("yyyy-MM-dd HH:mm:ss") },
                {"pageIndex",1 }
            };

            while (pageIndex <= totalPage)
            {
                data[nameof(pageIndex)] = pageIndex;
                order? order = this.Post<order>(APIMethod.GetOrder, "/data/merchant/betHistoryRecord/v1", data, out GameResultCode code, out string message,
                    server: this.LogServer);
                if (code != GameResultCode.Success)
                {
                    return new OrderResult(code)
                    {
                        Message = message
                    };
                }

                foreach (var item in order.record)
                {
                    string rawData = item.ToJson();
                    GameAPIOrderStatus status = GameAPIOrderStatus.Wait;
                    switch (item.betStatus)
                    {
                        case 0:
                            status = GameAPIOrderStatus.Wait;
                            break;
                        case 1:
                            if (item.netAmount > 0)
                            {
                                status = GameAPIOrderStatus.Win;
                            }
                            else if (item.netAmount < 0)
                            {
                                status = GameAPIOrderStatus.Lose;
                            }
                            else
                            {
                                status = GameAPIOrderStatus.Return;
                            }
                            break;
                        case 2:
                            status = GameAPIOrderStatus.Return;
                            break;
                    }
                    result.data.Add(new OrderData()
                    {
                        orderId = item.id,
                        playerName = item.playerName,
                        betMoney = item.betAmount,
                        betAmount = item.validBetAmount,
                        money = item.netAmount,
                        createTime = item.createdAt,
                        settleTime = item.netAt,
                        gameCode = item.gameTypeId.ToString(),
                        currency = currency.Get(item.currency, GameCurrency.CNY),
                        rawData = rawData,
                        hash = item.updatedAt.ToString(),
                        status = status
                    });
                }

                pageIndex++;
                totalPage = order.totalPage;
            }

            return result;
        }

        public override LoginResponse Login(LoginModel request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "loginName",request.PlayerName },
                {"loginPassword",request.PlayerPassword },
                {"deviceType", this.getPlatform(request.Platform) },
                {"lang",1 },
                {"backurl","https://localhost" },
                {"ip",request.clientIp }
            };
            login? login = this.Post<login>(APIMethod.Login, "/api/merchant/fastGame/v2", data, out GameResultCode code, out string message);
            return new LoginResponse(code)
            {
                Message = message,
                Method = LoginMethod.Redirect,
                Url = login?.url
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            for (int index = 0; index < 5; index++)
            {
                string playerName = this.merchantCode + this.CreateUserName(request.Prefix, request.UserName, tryCount: index);
                string loginPassword = this.GetType().Name.ToLower();
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"loginName",playerName },
                    {"loginPassword",loginPassword },
                    {"lang",1 }
                };
                register? register = this.Post<register>(APIMethod.Register, "/api/merchant/create/v2", data, out GameResultCode code, out string message);
                if (code == GameResultCode.DuplicatePlayerName) continue;
                return new RegisterResponse(code)
                {
                    PlayerName = playerName,
                    Password = loginPassword,
                    Message = message
                };
            }
            return new RegisterResponse(GameResultCode.DuplicatePlayerName);
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            string transferId = $"{WebAgent.GetTimestamp(DateTime.Now)}{request.OrderID}";
            string path = request.Money > 0 ? "/api/merchant/deposit/v1" : "/api/merchant/withdraw/v1";
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"loginName", request.PlayerName },
                {"transferNo",transferId},
                {"amount",Math.Abs(request.Money) },
                {"showBalance",1 }
            };
            transfer? transfer = this.Post<transfer>(APIMethod.Transfer, path, data, out GameResultCode code, out string message);

            return new TransferResponse(code)
            {
                Balance = transfer?.balance,
                Money = Math.Abs(request.Money),
                OrderID = request.OrderID,
                PlayerName = request.PlayerName,
                TransferID = transferId,
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
            response? response = JsonConvert.DeserializeObject<response>(result);
            if (response == null)
            {
                message = result;
                return GameResultCode.Exception;
            }
            message = response.message;
            return response.code switch
            {
                "200" => GameResultCode.Success,
                "20000" => GameResultCode.DuplicatePlayerName,
                "30007" => GameResultCode.OrderNotFound,
                "90003" => GameResultCode.ParameterInvalid,
                "99999" => GameResultCode.Error,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            request.Option.Headers.Add("Content-Type", "application/json");
            using (HttpClient client = new HttpClient())
            {
                return client.Post(request.Url, request.Data, request.Option.Headers);
            }
        }

        #region ========  工具方法  ========

        private T? Post<T>(APIMethod method, string path, Dictionary<string, object> data, out GameResultCode code, out string message, string? server = null) where T : class
        {
            server ??= this.gateway;
            string url = $"{server}{path}";
            int pageIndex = (int)data.Get(nameof(pageIndex), 0);
            data.Add("timestamp", WebAgent.GetTimestamps());
            string source = JsonConvert.SerializeObject(data);
            data.Clear();
            data.Add(nameof(merchantCode), this.merchantCode);
            data.Add("params", EncryptAgent.AesEncrypt(source, this.aesKey));
            data.Add("signature", EncryptAgent.toMD5(source + this.md5Key).ToLower());

            GameResponse result = this.Request(new GameRequest()
            {
                Url = url,
                Data = JsonConvert.SerializeObject(data),
                Method = method,
                Option = new Dictionary<string, string>()
                {
                    {nameof(merchantCode), this.merchantCode },
                    {nameof(pageIndex),pageIndex.ToString() }
                }
            });
            code = result.Code;
            message = result.Message;
            response<T>? response = JsonConvert.DeserializeObject<response<T>>(result);
            return response?.data;
        }

        /// <summary>
        /// 获取平台代码
        /// </summary>
        private int getPlatform(GamePlatform platform)
        {
            //1   网页
            //2   手机网页
            //3   App iOS或 h5 iOS
            //4   App Android 或h5 Android
            //5   其他设备
            //6   移动端 横竖合一
            //7   移动端 横竖合一横版
            //8   移动端 横竖合一竖版
            if (platform.HasFlag(GamePlatform.Mobile)) return 2;
            return 1;
        }

        #endregion

        #region ========  实体类  ========

        class response
        {
            public string code { get; set; }

            public string message { get; set; }
        }

        class response<T>
        {
            public string code { get; set; }

            public string message { get; set; }

            public T data { get; set; }

            public Dictionary<string, object> request { get; set; }
        }

        class register
        {
            public string loginName { get; set; }

            public string loginPassword { get; set; }
        }

        class login
        {
            public string url { get; set; }
        }

        /// <summary>
        /// 余额
        /// </summary>
        class balanceResponse
        {
            public decimal balance { get; set; }
        }

        class transfer
        {
            public string withdraw { get; set; }

            public string deposit { get; set; }

            public decimal balance { get; set; }
        }

        class checkTransfer
        {
            public string tradeNo { get; set; }

            public decimal amount { get; set; }

            /// <summary>
            /// 0成功 1失败 2转账中。
            /// </summary>
            public int transferStatus { get; set; }

            public string remark { get; set; }
        }

        class order
        {
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            public int totalRecord { get; set; }
            public int totalPage { get; set; }

            public orderRecord[] record { get; set; }
        }

        class orderRecord
        {

            //"id":412398064816185345,
            //"playerId":422776,
            //"playerName":"j7l6joy5test000000386",
            //"agentId":2531,
            //"betAmount":10,
            //"validBetAmount":7,
            //"netAmount":15,
            //"beforeAmount":485,
            //"createdAt":1578489825000,
            //"netAt":1578489881000,
            //"recalcuAt":1578489825000,
            //"updatedAt":1578489883000,
            //"gameTypeId":2009,
            //"platformId":1,
            //"platformName":"国际厅",
            //"betStatus":1,
            //"betFlag":0,
            //"betPointId":3802,
            //"odds":0.95,
            //"judgeResult":"fjkjfkjsdfjkjsdfkjs",
            //"currency":"CNY",
            //"tableCode":"101",
            //"roundNo":"ZVF620117EXX",
            //"bootNo":"测试持续派彩20200108-2053",
            //"loginIp":"/0:0:0:0:0:",
            //"deviceType":1,
            //"deviceId":"25BBDCD06C32D477F7FA1C3E4A91B032",
            //"recordType":1,
            //"gameMode":0,
            //"signature":"AEBC13309ABEFE91C5389C5F0319872B",
            //"payAmount":0,
            //"startid":412398064816185345,
            //"realDeductAmount":10,
            //"bettingRecordType":1

            public string id { get; set; }

            /// <summary>
            /// 玩家编号
            /// </summary>
            public int playerId { get; set; }

            /// <summary>
            /// 玩家账号
            /// </summary>
            public string playerName { get; set; }

            /// <summary>
            /// 商户编号
            /// </summary>
            public int agentId { get; set; }

            /// <summary>
            /// 投注额
            /// </summary>
            public decimal betAmount { get; set; }

            /// <summary>
            /// 有效投注额
            /// </summary>
            public decimal validBetAmount { get; set; }

            /// <summary>
            /// 输赢额
            /// </summary>
            public decimal netAmount { get; set; }

            /// <summary>
            /// 下注前余额
            /// </summary>
            public decimal beforeAmount { get; set; }

            /// <summary>
            /// 投注时间
            /// </summary>
            public long createdAt { get; set; }

            /// <summary>
            /// 结算时间
            /// </summary>
            public long netAt { get; set; }

            /// <summary>
            /// 注单重新结算时间
            /// </summary>
            public long recalcuAt { get; set; }

            /// <summary>
            /// 更新时间
            /// </summary>
            public long updatedAt { get; set; }

            /// <summary>
            /// 游戏编码
            /// </summary>
            public int gameTypeId { get; set; }

            /// <summary>
            /// 厅id
            /// </summary>
            public int platformId { get; set; }

            /// <summary>
            /// 厅名称
            /// </summary>
            public string platformName { get; set; }

            /// <summary>
            /// 注单状态
            /// 0=未结算 1=已结算 2=取消投注
            /// 只能获取到已结算的下注记录。且不包含任何试玩数据。
            /// </summary>
            public int betStatus { get; set; }

            /// <summary>
            /// 重算标志
            /// 普通注单(0=正常结算，1=跳局，2=取消局，4=重算指定局 )
            /// 大赛注单(0=正常, 1=跳局, 2=比赛取消 ,3=取消局 ,4=重算局 ,5=已弃赛)
            /// </summary>
            public int betFlag { get; set; }

            /// <summary>
            /// 玩法，下注点
            /// </summary>
            public int betPointId { get; set; }

            /// <summary>
            /// 赔率
            /// </summary>
            public string odds { get; set; }

            /// <summary>
            /// 局结果
            /// </summary>
            public string judgeResult { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 桌台号
            /// </summary>
            public string tableCode { get; set; }

            /// <summary>
            /// 局号
            /// </summary>
            public string roundNo { get; set; }

            /// <summary>
            /// 靴号 百家乐/龙虎/21点游戏有靴号，其他游戏该参数为空。
            /// </summary>
            public string bootNo { get; set; }

            /// <summary>
            /// 游戏ip
            /// </summary>
            public string loginIp { get; set; }

            /// <summary>
            /// 设备类型
            /// </summary>
            public int deviceType { get; set; }

            /// <summary>
            /// 设备id
            /// </summary>
            public string deviceId { get; set; }

            /// <summary>
            /// 注单类别    1、正式 
            /// </summary>
            public int recordType { get; set; }

            /// <summary>
            /// 游戏模式 
            /// 0=常规下注  1=入座 2=旁注    3=好路    4=多台
            /// </summary>
            public string gameMode { get; set; }

            /// <summary>
            /// 签名
            /// </summary>
            public string signature { get; set; }

            /// <summary>
            /// 返奖额 返奖额=投注额+输赢额
            /// </summary>
            public decimal payAmount { get; set; }

            /// <summary>
            /// 真实扣款金额
            /// </summary>
            public decimal realDeductAmount { get; set; }

            /// <summary>
            /// 注单类型	1-普通注单 2-大赛注单
            /// </summary>
            public int bettingRecordType { get; set; }
        }

        #endregion
    }
}
