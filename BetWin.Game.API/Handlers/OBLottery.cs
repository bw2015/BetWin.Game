using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using Newtonsoft.Json;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetWin.Game.API.Handlers
{
    internal class OBLottery : GameBase
    {
        [Description("网关")]
        public string gateway { get; set; }

        [Description("商户号")]
        public string merchant { get; set; }

        [Description("密钥")]
        public string key { get; set; }

        [Description("订单接口")]
        public string Order { get; set; }

        public OBLottery(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>
        {
            {GameLanguage.CHN,"1" },
            {GameLanguage.THN,"2" },
            {GameLanguage.ENG,"3" },
            {GameLanguage.VI,"4" }
        };

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>
        {
            {GameCurrency.CNY,"1" },
            {GameCurrency.USD,"2" },
            {GameCurrency.SGD,"3" },
            {GameCurrency.MYR,"4" },
            {GameCurrency.TWD,"5" },
            {GameCurrency.VND,"6" },
            {GameCurrency.BND,"7" }
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
                {"member",request.PlayerName },
                {"password",request.PlayerPassword },
                {"merchant",this.merchant },
                {"timestamp",WebAgent.GetTimestamps() },
            };
            data["sign"] = getSign(data);
            data["lang"] = this.Languages.Get(request.Language);
            data["loginIp"] = request.clientIp;
            response<login>? login = this.Post<login>(APIMethod.Login, "/boracay/api/member/login", data, out GameResultCode code, out string message);
            return new LoginResponse(code)
            {
                Message = message,
                Method = LoginMethod.Redirect,
                Url = request.Platform == GamePlatform.Mobile ? login?.data?.h5 : login?.data?.pc
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        protected override char UserNameSplit => '0';

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
            response? response = result.ToJson<response>();
            if (response == null)
            {
                message = result;
                return GameResultCode.Exception;
            }

            message = response.msg;
            return response.code switch
            {
                0 => GameResultCode.Success,
                401 => GameResultCode.SignInvalid,
                707 => GameResultCode.SignInvalid,
                405 => GameResultCode.ParameterInvalid,
                718 => GameResultCode.ParameterInvalid,
                701 => GameResultCode.DuplicatePlayerName,
                709 => GameResultCode.LoginFaild,
                726 => GameResultCode.LoginFaild,
                714 => GameResultCode.TransferFalid,
                715 => GameResultCode.TransferFalid,
                733 => GameResultCode.TransferFalid,
                716 => GameResultCode.TransferDuplicate,
                720 => GameResultCode.NoMerchant,
                727 => GameResultCode.PlayerLocked,
                730 => GameResultCode.SiteLock,
                777 => GameResultCode.SystemBuzy,
                1001 => GameResultCode.NoBalance,
                1003 => GameResultCode.TransferInvalid,
                _ => GameResultCode.Error
            };
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                return client.Post(request.Url, request.Data, new Dictionary<string, string>()
                {
                    {"Content-Type","application/json" }
                });
            }
        }

        #region ========  工具方法  ========

        private response<T>? Post<T>(APIMethod method, string path, Dictionary<string, object> data, out GameResultCode code, out string message)
        {
            string url = $"{this.gateway}{path}";
            GameResponse result = this.Request(new GameRequest()
            {
                Url = url,
                Data = data.ToJson(),
                Method = method
            });
            code = (GameResultCode)result;
            message = result.Message;

            return result.Content.ToJson<response<T>>();
        }

        private string getSign(Dictionary<string, object> data)
        {
            string value = string.Empty;
            foreach (var item in data.OrderBy(c => c.Key))
            {
                value += $"{item.Key}{item.Value}";
            }
            value += this.key;
            return value.toMD5().ToLower();
        }

        private string getSign(string value)
        {
            return value.toMD5().ToLower();
        }

        #endregion

        #region ========  实体类  ========

        class response
        {
            public int code { get; set; }

            public string msg { get; set; }
        }

        class response<T>
        {
            public int code { get; set; }

            public string msg { get; set; }

            public bool success { get; set; }

            public T data { get; set; }

            public static implicit operator T(response<T> response)
            {
                if (response == null) return default;
                return response.data;
            }
        }

        class login
        {

            /// <summary>
            /// C 端首页 web 地址 url
            /// </summary>
            public string pc { get; set; }

            /// <summary>
            /// C 端首页 h5 地址 url
            /// </summary>
            public string h5 { get; set; }

        }

        class balance
        {
            public string member { get; set; }

            public decimal amount { get; set; }
        }

        class transferCheck
        {
            public int total { get; set; }
            public transferCheckItem[] list { get; set; }
        }

        class transferCheckItem
        {
            public string id { get; set; }

            public string member { get; set; }

            /// <summary>
            /// 帐变类型 ：转账类型 1 资金转入，2资金转出
            /// </summary>
            public int tradeType { get; set; }

            /// <summary>
            /// 账变金额
            /// </summary>
            public decimal amount { get; set; }

            /// <summary>
            /// 账变前，会员钱包余额，保留 4 位小数
            /// </summary>
            public decimal balanceBefore { get; set; }

            /// <summary>
            /// 账变后，会员钱包余额，保留 4 位小数
            /// </summary>
            public decimal balanceAfter { get; set; }

            /// <summary>
            /// 账变时间
            /// </summary>
            public DateTime tradeTime { get; set; }
        }

        class order
        {
            /*
            {
             "orderId": 1207929569328234500, //注单 id
             "playId": 227010101, //玩法 id
             "playItemId": 2701010101, //投注项 id
             "ticketId": 27, //彩种 id
             "playLevelId": 270101, //玩法群 id
             "ticketPlanNo": "2019141", //奖期号
             "memberId": 1203344244085883000, //会员 id
             "memberAccount": "yyyybbb", //会员账号
             "betMoney": 10, //投注金额
             "seriesId": 3, //彩系 id
             "seriesName": "六合彩" //彩系名称
             "betTime": "2019-12-20T15:43:46",//投注时间
             "betNums": 1, //注数
             "betMultiple": 10, //倍数
             "betModel": 1, //投注模式：1, 0.1, 0.01, 0.001, 2, 0.2, 0.02, 0.002
             "betRebate": 0.5000, //投注返点
             "betPrize": "{"1":"19.1400"}", //奖金：多奖级逗号隔开
             "odd": "{"1":"1.9140"}", //赔率：多奖级逗号隔开
             "betStatus": 1,//投注状态：1-待开奖,2-未中 奖,3-已中奖,4-挂起,5-已结算
             "cancelStatus":false , /撤单标志.false：未撤单 true：已撤单.
             "cancelType": 0, //撤单类型：1-个人撤单,2-系统撤单,3:中奖停追撤单;4：不中停追撤单.
             "riskStatus":1,//状态 1 待风控,2 风控通过,3 风控锁定,4 风控解锁.
             "riskUnlockBy":"", //风控解锁人
             "riskUnlockAt":"", //风控解锁时间
            "ticketName": "香港六合彩", //彩种名
             "playLevel": "双面", //玩法群名
             "playName": "特码大小", //玩法名
             "singleGame": true, //是否单式玩法：false：否 true：是
             "baseRate": "{"1":"92.75%"}", //基准返奖率
            "bonusReduceRate": "{"1":"5.0"}", //返奖率下调幅度
             "directlyMode": false, //结算模式，false.代理，true.直客
             "memberRebate": 0.0500, // 用户返点值
             "ticketResult":"", //开奖结果
             "winAmount": 0, //中奖金额
            "winNums": 0, //中奖注数
             "solo": true, // 是否单挑 false: 否, true: 是
             "chaseId":0, //追号单 id
             "chasePlanId":0, //追号排期表 ID
             "groupMode": 2, //盘面 1:标准盘,2:双面盘 3:特色.
             "device": 1, //注终端：1-web,2-IOS,3-Android,4-H5
             "seriesType": false, //系列类型: false 传统 true 特色
             "chaseOrder": false, //注单类型: false.普通注单,true.追号注单.
             "cancelBy":"", //注单撤销人 account
             "cancelAt":"", //注单撤销时间
             "cancelDesc":"", //注单撤销说明
             "updateAt":" 2020-08-31T11:12:13", //注单最后更新时间
             "tester": false, //是否测试账户，false:否 true:是
             "betNum": "da", //投注号码
            "betContent": "大" , //前台投注内容
             "profitAmount": -10, //盈利金额 
             "merchantId": 2, //商户 id 
             "merchantAccount":"bob", //商户账号
             "currencyType":1 //币种类型 1,cny 人民币 2,usd 美元 3,sgd 新加坡元 4,myr 马来西亚林吉特 5,twd 新台币 6,vnd 越南盾 7,bnd 文莱林吉特
            }
            */

            /// <summary>
            /// /注单 id
            /// </summary>
            public long orderId { get; set; }

            /// <summary>
            /// /玩法 id
            /// </summary>
            public int playId { get; set; }

            /// <summary>
            /// 投注项 id
            /// </summary>
            public long playItemId { get; set; }

            /// <summary>
            /// /彩种 id
            /// </summary>
            public int ticketId { get; set; }

            /// <summary>
            /// 玩法群 id
            /// </summary>
            public int playLevelId { get; set; }

            /// <summary>
            /// 奖期号
            /// </summary>
            public string ticketPlanNo { get; set; }

            /// <summary>
            /// 会员 id
            /// </summary>
            public long memberId { get; set; }

            /// <summary>
            /// 会员账号
            /// </summary>
            public string memberAccount { get; set; }

            /// <summary>
            /// 投注金额
            /// </summary>
            public decimal betMoney { get; set; }

            /// <summary>
            /// 彩系 id
            /// </summary>
            public int seriesId { get; set; }

            /// <summary>
            /// 彩系名称
            /// </summary>
            public string seriesName { get; set; }

            /// <summary>
            /// 投注时间（北京时间）
            /// </summary>
            public DateTime betTime { get; set; }

            /// <summary>
            /// 注数
            /// </summary>
            public int betNums { get; set; }

            /// <summary>
            /// 倍数
            /// </summary>
            public int betMultiple { get; set; }

            /// <summary>
            /// 投注模式    1, 0.1, 0.01, 0.001, 2, 0.2, 0.02, 0.002
            /// </summary>
            public decimal betModel { get; set; }

            /// <summary>
            /// 投注返点
            /// </summary>
            public decimal betRebate { get; set; }

            /// <summary>
            /// 奖金：多奖级逗号隔开
            /// </summary>
            public string betPrize { get; set; }

            /// <summary>
            /// 赔率：多奖级逗号隔开
            /// </summary>
            public string odd { get; set; }

            /// <summary>
            /// 投注状态：1-待开奖,2-未中奖,3-已中奖,4-挂起,5-已结算
            /// </summary>
            public int betStatus { get; set; }

            /// <summary>
            /// 撤单标志.false：未撤单 true：已撤单.
            /// </summary>
            public bool cancelStatus { get; set; }

            /// <summary>
            /// 撤单类型：1-个人撤单,2-系统撤单,3:中奖停追撤单;4：不中停追撤单.
            /// </summary>
            public int cancelType { get; set; }

            /// <summary>
            /// 风控状态 1 待风控,2 风控通过,3 风控锁定,4 风控解锁.
            /// </summary>
            public int riskStatus { get; set; }

            /// <summary>
            /// 风控解锁人
            /// </summary>
            public string riskUnlockBy { get; set; }

            /// <summary>
            /// 风控解锁时间
            /// </summary>
            public DateTime? riskUnlockAt { get; set; }

            /// <summary>
            /// 彩种名
            /// </summary>
            public string ticketName { get; set; }

            /// <summary>
            /// 玩法群名
            /// </summary>
            public string playLevel { get; set; }

            /// <summary>
            /// 玩法名
            /// </summary>
            public string playName { get; set; }

            /// <summary>
            /// 是否单式玩法
            /// </summary>
            public bool singleGame { get; set; }

            /// <summary>
            /// 基准返奖率
            /// </summary>
            public string baseRate { get; set; }

            /// <summary>
            /// 返奖率下调幅度
            /// </summary>
            public string bonusReduceRate { get; set; }

            /// <summary>
            /// 结算模式，false.代理，true.直客
            /// </summary>
            public bool directlyMode { get; set; }

            /// <summary>
            /// 用户返点值
            /// </summary>
            public decimal memberRebate { get; set; }

            /// <summary>
            /// 开奖结果
            /// </summary>
            public string ticketResult { get; set; }

            /// <summary>
            /// 中奖金额
            /// </summary>
            public decimal winAmount { get; set; }

            /// <summary>
            /// 中奖注数
            /// </summary>
            public decimal winNums { get; set; }

            /// <summary>
            /// 是否单挑 false: 否, true: 是
            /// </summary>
            public bool solo { get; set; }

            /// <summary>
            /// 追号单 id
            /// </summary>
            public long chaseId { get; set; }

            /// <summary>
            /// 追号排期表 ID
            /// </summary>
            public long chasePlanId { get; set; }

            /// <summary>
            /// 盘面 1:标准盘,2:双面盘 3:特色.
            /// </summary>
            public int groupMode { get; set; }

            /// <summary>
            /// 注终端：1-web,2-IOS,3-Android,4-H5
            /// </summary>
            public int device { get; set; }

            /// <summary>
            /// 系列类型: false 传统 true 特色
            /// </summary>
            public bool seriesType { get; set; }

            /// <summary>
            /// 注单类型: false.普通注单,true.追号注单
            /// </summary>
            public bool chaseOrder { get; set; }

            /// <summary>
            /// 注单撤销人 account
            /// </summary>
            public string cancelBy { get; set; }

            /// <summary>
            /// 注单撤销时间
            /// </summary>
            public DateTime? cancelAt { get; set; }

            /// <summary>
            /// 注单撤销说明
            /// </summary>
            public string cancelDesc { get; set; }

            /// <summary>
            /// 注单最后更新时间
            /// </summary>
            public DateTime updateAt { get; set; }

            /// <summary>
            /// 是否测试账户，false:否 true:是
            /// </summary>
            public bool tester { get; set; }

            /// <summary>
            /// 投注号码
            /// </summary>
            public string betNum { get; set; }

            /// <summary>
            /// 前台投注内容
            /// </summary>
            public string betContent { get; set; }

            /// <summary>
            /// 盈利金额
            /// </summary>
            public decimal profitAmount { get; set; }

            /// <summary>
            /// 商户 id 
            /// </summary>
            public int merchantId { get; set; }

            /// <summary>
            /// 商户账号
            /// </summary>
            public string merchantAccount { get; set; }

            /// <summary>
            /// 币种类型 1,cny 人民币 2,usd 美元 3,sgd 新加坡元 4,myr 马来西亚林吉特 5,twd 新台币 6,vnd 越南盾 7,bnd 文莱林吉特
            /// </summary>
            public int currencyType { get; set; }

            /// <summary>
            /// 订单的唯一Hash
            /// </summary>
            /// <returns></returns>
            public string GetHash()
            {
                return string.Concat(this.orderId, ":", WebAgent.GetTimestamps(this.updateAt));
            }

        }

        #endregion
    }
}
