using BetWin.Game.Payment.Enums;
using BetWin.Game.Payment.Models;
using BetWin.Game.Payment.Utils;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X9;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using static BetWin.Game.Payment.Withdraws.PayCloud;

namespace BetWin.Game.Payment.Withdraws
{
    [Description("极付云")]
    public class PayCloud : WithdrawProviderBase
    {
        [Description("网关")]
        public string gateway { get; set; } = "http://115.126.121.25:6005";

        [Description("key")]
        public string apiKey { get; set; }

        [Description("公钥")]
        public string publicKey { get; set; }

        [Description("商户ID")]
        public string merchantId { get; set; }

        /// <summary>
        /// 1支付宝转卡，2银行卡转卡，3微信转卡
        /// </summary>
        [Description("渠道编号")]
        public string ditchId { get; set; }


        public PayCloud(string setting) : base(setting)
        {
        }

        /// <summary>
        /// 发起提现
        /// </summary>
        public override WithdrawResponse Withdraw(WithdrawRequest request)
        {
            OutMoneyOrder outMoneyOrder = new OutMoneyOrder()
            {
                identifier = request.orderId,//您的订单号
                sum = (int)(request.amount * 100),//单位：分
                merchantId = this.merchantId,
                ditchId = 2,
                payee = request.bankName ?? string.Empty,
                account = request.account ?? string.Empty,
                city = "",
                bankName = request.bankCode.ToString(),
                userId = request.userName ?? string.Empty,
                userlevel = 6,
                usePayType = UsePayTypeEnum.BankCard,
                customUrl = "",
                orderFlag = "",//订单标识，如一个商户多个平台，可以传平台的标识用于区别订单是那个平台，对平台的订单数据进行统计与区分等
                universalparameters = "",
                transparentValue = "",
            };

            string url = $"{this.gateway}/api/Order/OutMoneyOrder";

            string strJson = JsonConvert.SerializeObject(outMoneyOrder);
            var netPublicKey = new RsaHelper().RSAPublicKeyJava2DotNet(this.publicKey);
            RsaHelper rsaHelper = new RsaHelper(netPublicKey, null, false);
            Dictionary<string, string> pairs = new Dictionary<string, string>
            {
                {"appId", this.merchantId.ToString()},
                {"timestamp",WebAgent.GetTimestamps().ToString()},
                {"type",((int)MessageType.OutMoneyOrder).ToString()},
                {"state",((int)MessageState.Succeed).ToString()},
                {"identifier",outMoneyOrder.identifier},
                {"merchantId",this.merchantId.ToString()},
                {"text",rsaHelper.Encrypt(strJson)},
            };

            Dictionary<string, string> md5Pairs = pairs.OrderBy(c => c.Key).ToDictionary(x => x.Key, x => x.Value);
            md5Pairs.Add("apikey", this.apiKey);
            //签名第二步：把字典数据拼接成字符串
            var signData = this.CreateLinkString(md5Pairs);   //签名第三步：获取签名
            var sign = rsaHelper.md5Signature(signData);
            md5Pairs.Add("sign", sign);
            md5Pairs.Remove("apikey");

            HttpClient client = new HttpClient();
            HttpClientResponse response = client.Post(url, this.getPostData(md5Pairs), new Dictionary<string, string>()
            {
                {"Content-Type","application/x-www-form-urlencoded" }
            });
            if (!response)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            var result = JsonConvert.DeserializeObject<ResponseResult<CommunicationMessage>>(response);

            if (result != null && result.status == 200)
            {
                //接口请求成功业务逻辑
                CommunicationMessage communicationMessage = result.data;
                string strErrorText = communicationMessage.errorText;
                if (communicationMessage.state == MessageState.Succeed)
                {
                    communicationMessage.text = rsaHelper.Decrypt(communicationMessage.text);
                }

                return new WithdrawResponse()
                {
                    status = WithdrawProviderStatus.SubmitSuccess,
                    currency = PaymentCurrency.CNY,
                    msg = strErrorText,
                    amount = request.amount,
                    orderId = request.orderId,
                    tradeNo = request.orderId
                };
            }
            else
            {
                return new WithdrawResponse()
                {
                    status = WithdrawProviderStatus.SubmitFaild,
                    amount = request.amount,
                    orderId = request.orderId,
                    currency = PaymentCurrency.CNY,
                    msg = result?.data?.errorText ?? result?.msg ?? "null"
                };
            }
        }

        public override WithdrawQueryResponse Query(WithdrawQuery request)
        {
            var queryOrder = new
            {
                identifier = request.orderId,//您的订单号
                merchantId = this.merchantId,
                orderId = request.orderId//您的订单号
            };

            string url = $"{this.gateway}/api/Order/SelectOrder";

            string strJson = JsonConvert.SerializeObject(queryOrder);
            var netPublicKey = new RsaHelper().RSAPublicKeyJava2DotNet(this.publicKey);
            RsaHelper rsaHelper = new RsaHelper(netPublicKey, null, false);
            Dictionary<string, string> pairs = new Dictionary<string, string>
            {
                {"appId", this.merchantId.ToString()},
                {"timestamp",WebAgent.GetTimestamps().ToString()},
                {"type",((int)MessageType.SelectOrder).ToString()},
                {"state",((int)MessageState.Succeed).ToString()},
                {"identifier",queryOrder.identifier},
                {"merchantId",this.merchantId.ToString()},
                {"text",rsaHelper.Encrypt(strJson)},
            };

            Dictionary<string, string> md5Pairs = pairs.OrderBy(c => c.Key).ToDictionary(x => x.Key, x => x.Value);
            md5Pairs.Add("apikey", this.apiKey);
            //签名第二步：把字典数据拼接成字符串
            var signData = this.CreateLinkString(md5Pairs);   //签名第三步：获取签名
            var sign = rsaHelper.md5Signature(signData);
            md5Pairs.Add("sign", sign);
            md5Pairs.Remove("apikey");

            HttpClient client = new HttpClient();
            HttpClientResponse response = client.Post(url, this.getPostData(md5Pairs), new Dictionary<string, string>()
            {
                {"Content-Type","application/x-www-form-urlencoded" }
            });

            ResponseResult<CommunicationMessage>? result = JsonConvert.DeserializeObject<ResponseResult<CommunicationMessage>>(response);

            if (result != null && result.status == 200 && result?.data != null)
            {
                //接口请求成功业务逻辑
                CommunicationMessage communication = result.data;
                var messageState = communication.state;
                if (communication.state == MessageState.Succeed)
                {
                    communication.text = rsaHelper.Decrypt(communication.text);
                    var orderResponse = JsonConvert.DeserializeObject<QueryOrderResponse>(communication.text);
                    if (orderResponse != null)
                    {
                        return new WithdrawQueryResponse()
                        {
                            Currency = PaymentCurrency.CNY,
                            OrderID = request.orderId,
                            TradeNo = request.orderId,
                            Status = orderResponse.orderInfo.orderState switch
                            {
                                OrderStateEnum.Succeed => WithdrawProviderStatus.PaymentSuccess,
                                OrderStateEnum.CallbackSuccessful => WithdrawProviderStatus.PaymentSuccess,
                                OrderStateEnum.Callbackfeated => WithdrawProviderStatus.PaymentSuccess,

                                OrderStateEnum.Fail => WithdrawProviderStatus.PaymentFaild,

                                _ => WithdrawProviderStatus.None
                            }
                        };
                    }
                }
            }


            return new WithdrawQueryResponse()
            {
                OrderID = request.orderId,
                TradeNo = request.orderId,
                Currency = PaymentCurrency.CNY
            };
        }

        #region ========  工具方法  ========

        string CreateLinkString(Dictionary<string, string> sortedParams)
        {
            //为兼容httpHelper类特使用以下实现方式2021-05-30
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            foreach (var item in sortedParams)
            {
                if (item.Value == null)
                {
                    continue;
                }
                list.Add(item.Key + "=" + WebUtility.UrlEncode(item.Value));
            }
            return string.Join("&", list);
        }

        string getPostData(Dictionary<string, string> postData)
        {
            List<string> list = new List<string>();
            foreach (var item in postData)
            {
                if (item.Value == null)
                {
                    continue;
                }
                list.Add(item.Key + "=" + WebUtility.UrlEncode(item.Value));
            }
            return string.Join("&", list);
        }

        #endregion

        #region ========  实体类  ========

        class OutMoneyOrder
        {
            /// <summary>
            /// 消息的标识符，请求是什么返回就是什么，但必须是双方系统均为唯一标识符
            /// </summary>
            public string identifier { get; set; } = "";

            /// <summary>
            /// 收款金额，单位分
            /// </summary>
            public double sum { get; set; } = 0;

            /// <summary>
            /// 商户ID
            /// </summary>
            public string merchantId { get; set; } = "";

            /// <summary>
            /// 渠道ID,1支付宝转卡，2银行卡转卡，3微信转卡，4一对一支付
            /// </summary>
            public int ditchId { get; set; } = 2;

            /// <summary>
            /// 收款人姓名
            /// </summary>
            public string payee { get; set; } = string.Empty;

            /// <summary>
            /// 收款人卡号
            /// </summary>
            public string account { get; set; } = string.Empty;


            /// <summary>
            /// 帐号开户城市
            /// </summary>
            public string city { get; set; } = "深圳";

            /// <summary>
            /// 收款卡号银行简码:如工行为ICBC
            /// </summary>
            public string bankName { get; set; } = string.Empty;

            /// <summary>
            /// 用户ID
            /// </summary>
            public string userId { get; set; } = string.Empty;

            /// <summary>
            /// 用户层级
            /// </summary>
            public int userlevel { get; set; } = 1;

            /// <summary>
            /// 使用支付类型
            /// </summary>
            public UsePayTypeEnum usePayType { get; set; } = UsePayTypeEnum.BankCard;

            /// <summary>
            /// 自定义回调地址
            /// </summary>
            public string customUrl { get; set; } = string.Empty;

            /// <summary>
            /// 订单标识
            /// </summary>
            public string orderFlag { get; set; } = string.Empty;

            /// <summary>
            /// Desc:请求特定参数值
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string universalparameters { get; set; } = string.Empty;

            /// <summary>
            /// Desc:透传值，一般为一个json串，不同情况可以传不同值，提交过来的值，直接回传
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string transparentValue { get; set; } = string.Empty;
        }

        public enum UsePayTypeEnum
        {
            /// <summary>
            /// 支付宝转支付宝
            /// </summary>
            [Description("支付宝转支付宝")]
            Alipay = 1,

            /// <summary>
            /// 支付宝转银行卡
            /// </summary>
            [Description("支付宝转银行卡")]
            AlipayToBankCard = 2,

            /// <summary>
            /// 银行卡转银行卡
            /// </summary>
            [Description("银行卡转银行卡")]
            BankCard = 3,
            /// <summary>
            /// 微信转银行卡
            /// </summary>
            [Description("微信转银行卡")]
            WeChatToBankCard = 4,
            /// <summary>
            /// USDT
            /// </summary>
            [Description("USDT")]
            USDT = 5,
            /// <summary>
            /// 数字人民币
            /// </summary>
            [Description("数字人民币")]
            DCEP = 6,
            /// <summary>
            /// 数字人民币转银行卡
            /// </summary>
            [Description("数字人民币转银行卡")]
            DCEPToBankCard = 7
        }

        /// <summary>
        /// 消息状态
        /// </summary>
        public enum MessageState
        {
            /// <summary>
            ///成功
            /// </summary>
            [Description("成功")]
            Succeed = 1,
            /// <summary>
            /// 失败
            /// </summary>
            [Description("处理失败，程序发生异常，请联系软件平台技术人员处理与检查")]
            Failed = 2,
            #region 帐号类
            /// <summary>
            /// 账号类型不正确或当前商户状态不正确
            /// </summary>
            [Description("账号类型不正确或当前商户状态不正确")]
            AbnormalAccount = 101,

            /// <summary>
            /// 收款人名字不合法
            /// </summary>
            [Description("收款人名字不合法")]
            NameFailed = 102,
            #endregion　结束
            #region 卡类
            /// <summary>
            /// 没有查找到可用的卡，请上卡
            /// </summary>
            [Description("没有查找到可用的卡，请上卡")]
            NotBankCar = 201,

            /// <summary>
            /// 没有可以处理当前金额的卡，请换卡
            /// </summary>
            [Description("没有可以处理当前金额的卡，请换卡")]
            NoAmountAvailable = 202,


            /// <summary>
            /// 无可用卡池或无可分配卡，请检查卡池状态与相关配置，或者检查有无银行卡可分配
            /// </summary>
            [Description("无可用卡池或无可分配卡，请检查卡池状态与相关配置，或者检查有无银行卡可分配")]
            NoBankCardPool = 203,
            #endregion 结束卡类
            #region  订单类
            /// <summary>
            /// 当前使用的标识符订单ＩＤ，已经存在，请使用唯一的字符串UUIＤ为订单ID
            /// </summary>
            [Description("当前使用的标识符订单ID，已经存在，请使用唯一的字符串UUIＤ为订单ID")]
            OrderExist = 301,
            /// <summary>
            /// 当前使用的标识符订单ID，与订单对象的加密标识符不一致
            /// </summary>
            [Description("当前使用的标识符订单ID，与订单对象的加密标识符不一致")]
            IdentifierError = 302,
            /// <summary>
            /// 卡状态不正确或卡余额不够出款该笔订单
            /// </summary>
            [Description("订单结果异常，卡状态不正确或卡余额不够出款该笔订单")]
            ResultOrderError = 303,
            /// <summary>
            /// 订单保存失败
            /// </summary>
            [Description("订单保存失败")]
            SaveOrderError = 304,
            /// <summary>
            /// 订单小于系统的最小处理金额
            /// </summary>
            [Description("入款小于处理金额500元，出款小于1000元处理金额")]
            OrderMinSum = 305,
            /// <summary>
            /// 订单手工失败
            /// </summary>
            [Description("订单手工失败")]
            OrderFail = 306,
            #endregion 结束订单类

            #region 密钥异常
            /// <summary>
            /// 解密错误
            /// </summary>
            [Description("解密错误,请检查您的加密相关问题")]
            Declassification = 401,
            #endregion 结束密钥异常
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            ///入款订单
            /// </summary>
            [Description("入款订单")]
            InMoneyOrder = 1,
            /// <summary>
            /// 出款订单
            /// </summary>
            [Description("出款订单")]
            OutMoneyOrder = 2,

            /// <summary>
            /// 订单审核
            /// </summary>
            [Description("订单审核")]
            OrderPurchase = 3,
            /// <summary>
            /// 成功回调通知
            /// </summary>
            [Description("成功回调通知")]
            SuccessCallback = 4,

            /// <summary>
            /// 订单查询
            /// </summary>
            [Description("订单查询")]
            SelectOrder = 7,


            /// <summary>
            /// 查询余额
            /// </summary>
            [Description("查询余额")]
            SelectBalance = 11

        }

        public class CommunicationMessage
        {
            /// <summary>
            /// 消息状态
            /// </summary>
            public MessageState state { get; set; } = MessageState.Succeed;

            /// <summary>
            /// 消息的标识符，请求是什么返回就是什么，但必须是双方系统均为唯一标识符
            /// </summary>
            public string identifier { get; set; } = Guid.NewGuid().ToString();

            /// <summary>
            /// 消息的异常提示
            /// </summary>
            public string errorText => this.state.GetDescription();

            /// <summary>
            /// 消息的加密字符串
            /// </summary>
            public string text { get; set; } = string.Empty;

            /// <summary>
            /// 商户ID
            /// </summary>
            public int merchantId { get; set; } = 0;
        }

        class ResponseResult<T> where T : class
        {
            /// <summary>
            /// 状态:200-成功; 0-缺省;401-未授权; 500-失败
            /// </summary>
            public int status { get; set; }

            /// <summary>
            /// 返回消息内容
            /// </summary>
            public string msg { get; set; }

            /// <summary>
            /// 业务实体
            /// </summary>
            public T data { get; set; }
        }

        /// <summary>
        /// 查询订单返回实体对象
        /// </summary>
        class QueryOrderResponse
        {
            /// <summary>
            /// 订单对象
            /// </summary>
            public OrderInfo orderInfo { get; set; } = new OrderInfo();

            /// <summary>
            /// 银行明细对象
            /// </summary>
            public BankTrans trans { get; set; } = new BankTrans();
        }

        class OrderInfo
        {
            /// <summary>
            /// 订单ID
            /// </summary>
            public string orderId { get; set; } = string.Empty;

            /// <summary>
            /// 主订单号
            /// </summary>
            public string mainOrderId { get; set; } = string.Empty;

            /// <summary>
            /// 渠道ID
            /// </summary>
            public int ditchId { get; set; } = 0;
            /// <summary>
            /// 所在的商户ＩＤ
            /// </summary>
            public int merchantid { get; set; } = 0;

            /// <summary>
            /// 在那张银行卡上发生交易,出款为出款卡ID,入款为收款卡ID, 中转为出款卡ID
            /// </summary>
            public int bankCardId { get; set; } = 0;

            /// <summary>
            /// 银行简码
            /// </summary>
            public string bankCode { get; set; } = string.Empty;

            /// <summary>
            /// 收款人名字
            /// </summary>
            public string payee { get; set; } = string.Empty;

            /// <summary>
            /// 收款人账号
            /// </summary>
            public string payeeAccount { get; set; } = string.Empty;

            /// <summary>
            /// 收款人卡银联号id
            /// </summary>
            public int payeebankid { get; set; } = 0;

            /// <summary>
            /// 收款金额
            /// </summary>
            public double payeeSum { get; set; } = 0;

            /// <summary>
            /// 付款人名字
            /// </summary>
            public string payName { get; set; } = string.Empty;

            /// <summary>
            /// 付款人卡银联号id
            /// </summary>
            public int paybankid { get; set; } = 0;

            /// <summary>
            /// 付款人账号
            /// </summary>
            public string payAccount { get; set; } = string.Empty;

            /// <summary>
            /// 付款金额
            /// </summary>
            public double paySum { get; set; } = 0;

            /// <summary>
            /// 中转卡接收卡ID,该字段只对中转生效
            /// </summary>
            public int inBankid { get; set; } = 0;

            /// <summary>
            /// 交易方式0出款,1入款
            /// </summary>
            public int transType { get; set; } = 0;

            /// <summary>
            /// 订单类型:0出款订单,1入款订单，2中转订单，3测卡订单
            /// </summary>
            public int orderType { get; set; } = 0;

            /// <summary>
            /// 订单状态:0创建，1支付中，2成功，3失败
            /// </summary>
            public OrderStateEnum orderState { get; set; } = OrderStateEnum.Fail;

            /// <summary>
            /// 是否为自动订单 1:自动订单  2：手工订单
            /// </summary>
            public int isAuto { get; set; } = 1;

            /// <summary>
            /// 使用支付类型
            /// </summary>
            public UsePayTypeEnum usePayType { get; set; } = UsePayTypeEnum.BankCard;
            /// <summary>
            /// 订单备注信息，出款会填写出款表单备注项
            /// </summary>
            public string orderRemark { get; set; } = string.Empty;

            /// <summary>
            /// 订单备注1
            /// </summary>
            public string remark1 { get; set; } = string.Empty;

            /// <summary>
            /// 订单备注2
            /// </summary>
            public string remark2 { get; set; } = string.Empty;

            /// <summary>
            /// 订单创建时间
            /// </summary>
            public DateTime createDate { get; set; } = DateTime.Now;

            /// <summary>
            /// 开始处理时间
            /// </summary>
            public DateTime startDisposeDate { get; set; } = DateTime.Now;

            /// <summary>
            /// 订单完成时间，如果失败即为失败产生的时间
            /// </summary>
            public DateTime finishDate { get; set; } = DateTime.Now;

            /// <summary>
            /// 回调成功时间
            /// </summary>
            public DateTime callbackTime { get; set; } = DateTime.Now;

            /// <summary>
            /// 完成备注，失败即为失败原因
            /// </summary>
            public string finishRemark { get; set; } = string.Empty;

            /// <summary>
            /// 用户ID
            /// </summary>
            public string userId { get; set; } = string.Empty;

            /// <summary>
            /// 用户层级
            /// </summary>
            public int userlevel { get; set; } = 1;

            /// <summary>
            /// 是否正在进行手工出款中，0没有进行手工出款，1，正在进行手工出款，2完成手工出款
            /// </summary>
            public int isHandmadePaging { get; set; } = 0;

            /// <summary>
            /// 自定义回调Url地址
            /// </summary>
            public string customUrl { get; set; } = string.Empty;

            /// <summary>
            /// Desc:手续费，单位:分
            /// Default:0
            /// Nullable:True
            /// </summary>           
            public double handlingFee { get; set; } = 0;

            /// <summary>
            /// Desc:用户备注
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string userComments { get; set; } = string.Empty;

            /// <summary>
            /// Desc:透传值，一般为一个json串，不同情况可以传不同值，提交过来的值，直接回传
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string transparentValue { get; set; } = string.Empty;

            /// <summary>
            /// Desc:支付类型:1单数类型，2主支付，3子支付
            /// Default:1
            /// Nullable:True
            /// </summary>           
            public int orderPayType { get; set; } = 1;

            /// <summary>
            /// Desc:请求特定参数值
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string universalparameters { get; set; } = string.Empty;

        }

        enum OrderStateEnum
        {
            /// <summary>
            /// 创建订单
            /// </summary>
            [Description("创建订单")]
            Created = 0,
            /// <summary>
            /// 支付中
            /// </summary>
            [Description("支付中")]
            Paying = 1,
            /// <summary>
            /// 匹配成功
            /// </summary>
            [Description("匹配成功")]
            Succeed = 2,
            /// <summary>
            /// 处理超时
            /// </summary>
            [Description("处理超时")]
            Defeated = 3,
            /// <summary>
            /// 回调成功
            /// </summary>
            [Description("回调成功")]
            CallbackSuccessful = 4,
            /// <summary>
            /// 回调失败
            /// </summary>
            [Description("回调失败")]
            Callbackfeated = 5,
            /// <summary>
            /// 手工失败
            /// </summary>
            [Description("手工失败")]
            Fail = 6,
            /// <summary>
            /// 主支付
            /// </summary>
            [Description("主支付")]
            MainPay = 7,
            /// <summary>
            /// 子支付
            /// </summary>
            [Description("子支付")]
            ChildrenPay = 8,
            /// <summary>
            /// 子支付完成
            /// </summary>
            [Description("子支付完成")]
            ChildrenSuccess = 9
        }

        class BankTrans
        {
            /// <summary>
            /// 银行卡主键，自增主键
            /// </summary>
            public int bankCardId { get; set; } = 0;
            /// <summary>
            /// 交易流水号
            /// </summary>
            public string accountid { get; set; } = $"{Guid.NewGuid().ToString()}-";

            /// <summary>
            /// 银行卡简码:ABC025368-张三
            /// </summary>
            /// <remarks>基础数据</remarks>
            public string accountCode { get; set; } = string.Empty;
            /// <summary>
            /// 做了隐藏显示的对方交易账号如:112345*******12555
            /// </summary>
            public string hideAccount { get; set; } = string.Empty;

            /// <summary>
            /// 当前登录卡号
            /// </summary>
            public string loginAccount { get; set; }

            /// <summary>
            /// 交易时间
            /// </summary>
            public DateTime transTime { get; set; } = DateTime.Now.AddYears(-30);
            /// <summary>
            /// 交易方式:0支出,1:收入
            /// </summary>
            public int transType { get; set; } = 1;
            /// <summary>
            /// 交易类型中文说明
            /// </summary>
            public string transTypeName
            {
                get
                {
                    return transType == 1 ? "转入" : "转出";
                }
            }
            /// <summary>
            /// 交易对方账号
            /// </summary>
            public string account { get; set; } = string.Empty;
            /// <summary>
            /// 交易金额:单位分
            /// </summary>
            public double transAmount { get; set; } = 0.0;
            /// <summary>
            /// 帐户余额:单位分
            /// </summary>
            public double balance { get; set; } = 0.0;
            /// <summary>
            /// 备注
            /// </summary>
            public string remark { get; set; } = string.Empty;
            /// <summary>
            /// 交易姓名
            /// </summary>
            public string name { get; set; } = string.Empty;

            /// <summary>
            /// 上传时间 
            /// </summary>
            public string sendTime { get; set; } = string.Empty;

            /// <summary>
            /// 唯一标识
            /// </summary>
            public string unique { get; set; } = string.Empty;

        }

        #endregion
    }
}
