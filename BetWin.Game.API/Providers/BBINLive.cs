using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BetWin.Game.API.Providers
{
    [Description("BBIN真人")]
    public class BBINLive : BBINBase
    {
        protected override char UserNameSplit => '0';

        public BBINLive(string jsonString) : base(jsonString)
        {
        }

        public override LoginResponse Login(LoginModel request)
        {
            string? sessionId = this.getSessionId(request, out GameResultCode code, out string message);
            if (sessionId == null)
            {
                return new LoginResponse(code)
                {
                    Message = message
                };
            }

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"website",this.WebSite },
                {"sessionid",sessionId },
                {"tag","asia" },
                {"lang",Languages.Get(request.Language,"zh-cn") },
                {"key", getKey(6, string.Empty, GameUrlKey, 7) }
            };

            response<login[]>? login = this.get<login[]>(APIMethod.Login, "GameUrlBy3", data, out code, out message);

            return new LoginResponse(code)
            {
                Message = message,
                Method = LoginMethod.Redirect,
                Url = request.Platform == GamePlatform.PC ? login?.data?.FirstOrDefault()?.pc : login?.data?.FirstOrDefault()?.mobile
            };
        }

        public override int CollectDelay => 1000;

        DateTime beginTime => DateTime.Now.AddDays(-3);
        /// <summary>
        /// 当action为ModifiedTime，无法捞取最近2分钟内的下注记录。
        /// YYYYMMDD为美东时间(GMT-4)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override OrderResult GetOrder(QueryOrderModel request)
        {
            if (WebAgent.GetTimestamps(request.StartTime) < beginTime)
            {
                request.StartTime = WebAgent.GetTimestamps(beginTime);
            }

            int page = 1;
            DateTime now = DateTime.Now.AddHours(-12).AddMinutes(-2);

            DateTime starttime = WebAgent.GetTimestamps(request.StartTime).AddHours(-12);
            if (starttime.TimeOfDay == new TimeSpan(23, 59, 59))
            {
                starttime = starttime.AddDays(1).Date;
            }
            DateTime endtime = starttime.AddMinutes(5);

            DateTime date = starttime.Date;
            if (endtime.Date != date)
            {
                endtime = endtime.Date.AddSeconds(-1);
            }
            if (endtime >= now)
            {
                endtime = now;
                starttime = endtime.AddMinutes(-5);
                if (starttime.Date != endtime.Date) starttime = endtime.Date;
            }

            OrderResult result = new OrderResult(GameResultCode.Success)
            {
                startTime = WebAgent.GetTimestamps(starttime.AddHours(12)),
                endTime = WebAgent.GetTimestamps(endtime.AddHours(12)),
                data = new List<OrderData>()
            };
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"website",this.WebSite },
                {"action","ModifiedTime" },
                {"uppername",this.UpperName },
                {"date", date.ToString("yyyy-MM-dd")},
                {"starttime",starttime.TimeOfDay.ToString(@"hh\:mm\:ss") },
                {"endtime",endtime.TimeOfDay.ToString(@"hh\:mm\:ss") },
                {"page",page },
                {"pagelimit",10000 },
                {"key",this.getKey(4,string.Empty,WagersRecordKey,8) }
            };

            while (true)
            {
                response<order[]>? response = this.get<order[]>(APIMethod.GetOrder, "WagersRecordBy3", data, out GameResultCode code, out string message);

                if (code != GameResultCode.Success)
                {
                    return new OrderResult(code)
                    {
                        Message = message
                    };
                }
                int totalPage = response?.pagination?.TotalPage ?? 0;

                foreach (order order in response?.data ?? Array.Empty<order>())
                {
                    result.data.Add(new OrderData()
                    {
                        category = "Live",
                        playerName = order.UserName[this.Prefix.Length..],
                        status = this.ConvertOrderStatus(order.Result),
                        betAmount = order.Commissionable,
                        betMoney = order.BetAmount,
                        createTime = WebAgent.GetTimestamps(order.WagersDate.AddHours(12)),
                        currency = this.ConvertCurrency(order.Currency),
                        gameCode = order.GameType,
                        money = order.Payoff,
                        orderId = order.WagersID,
                        settleTime = WebAgent.GetTimestamps(order.ModifiedDate.AddHours(12)),
                        hash = $"{order.WagersID}:{order.ModifiedDate}",
                        rawData = order.ToJson(),
                        clientIp = ""
                    });
                }

                page++;
                if (page > totalPage) break;
            }

            return result;
        }


        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        #region ========  实体类  ========

        class order
        {
            /// <summary>
            /// 帐号
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 注单号码
            /// </summary>
            public string WagersID { get; set; }

            /// <summary>
            /// 下注时间
            /// </summary>
            public DateTime WagersDate { get; set; }

            /// <summary>
            /// 局号
            /// </summary>
            public string SerialID { get; set; }

            /// <summary>
            /// 场次
            /// </summary>
            public string RoundNo { get; set; }

            /// <summary>
            /// 游戏种类
            /// </summary>
            public string GameType { get; set; }

            /// <summary>
            /// 玩法
            /// </summary>
            public string WagerDetail { get; set; }

            /// <summary>
            /// 桌号
            /// </summary>
            public string GameCode { get; set; }

            /// <summary>
            /// 注单结果(C:注销,X:未结算,W:成功/赢,L:输,D:和局)
            /// </summary>
            public string Result { get; set; }

            /// <summary>
            /// 开牌结果
            /// </summary>
            public string ResultType { get; set; }

            /// <summary>
            /// 结果牌
            /// </summary>
            public string Card { get; set; }

            /// <summary>
            /// 下注金额
            /// </summary>
            public decimal BetAmount { get; set; }

            /// <summary>
            /// 派彩金额(不包含本金)
            /// </summary>
            public decimal Payoff { get; set; }

            /// <summary>
            /// 币别
            /// </summary>
            public string Currency { get; set; }

            /// <summary>
            /// 与人民币的汇率
            /// </summary>
            public decimal ExchangeRate { get; set; }

            /// <summary>
            /// 会员有效投注额
            /// </summary>
            public decimal Commissionable { get; set; }

            /// <summary>
            /// 退水(action=ModifiedTime时，才会回传)
            /// </summary>
            public string Commission { get; set; }

            /// <summary>
            /// 1.行动装置下单：M
            /// 1-1.ios手机：MI1
            /// 1-2.ios平板：MI2
            /// 1-3.Android手机：MA1
            /// 1-4.Android平板：MA2
            /// 2.计算机下单：P
            /// 3. MAC下单：MAC
            /// 4. 其他：O
            /// </summary>
            public string Origin { get; set; }

            /// <summary>
            /// 注单变更时间
            /// </summary>
            public DateTime ModifiedDate { get; set; }

            /// <summary>
            /// 开发平台(0:WEB, 1:APP, 2:Flash, 3:HTML5, 5:AIO)
            /// </summary>
            public int Client { get; set; }

            /// <summary>
            /// 来源入口(0:PC, 1:APP, 2:行动装置网页版, 3:AIO, 
            /// 4:AIOS, 5:AIO SDK, 6:UB+PC版, 7:UB+行动装置网页版, 
            /// 8:UB客制化+PC版, 9:UB客制化+行动装置网页版, 10:PWA, 11:其他)
            /// </summary>
            public int Portal { get; set; }
        }

        #endregion
    }
}
