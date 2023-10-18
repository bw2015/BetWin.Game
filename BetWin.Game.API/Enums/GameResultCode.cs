using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Enums
{
    /// <summary>
    /// 游戏接口返回的代码
    /// </summary>
    public enum GameResultCode
    {
        [Description("成功")]
        Success = 0,
        /// <summary>
        /// 参数不合法
        /// </summary>
        [Description("参数不合法")]
        ParameterInvalid = 1001,
        /// <summary>
        /// 验签失败
        /// </summary>
        [Description("验签失败")]
        SignInvalid = 1002,
        /// <summary>
        /// 没有此商户
        /// </summary>
        [Description("没有此商户")]
        NoMerchant = 1003,
        /// <summary>
        /// 没有此玩家
        /// </summary>
        [Description("没有此玩家")]
        NoPlayer = 1004,
        /// <summary>
        /// 玩家名称重复
        /// </summary>
        [Description("玩家名称重复")]
        DuplicatePlayerName = 1005,
        /// <summary>
        /// 订单号不存在
        /// </summary>
        [Description("订单号不存在")]
        OrderNotFound = 1006,
        /// <summary>
        /// 玩家被锁定
        /// </summary>
        [Description("玩家被锁定")]
        PlayerLocked = 1007,
        /// <summary>
        /// 用户名不合法
        /// </summary>
        [Description("用户名不合法")]
        PlayerNameInvalid = 1008,
        /// <summary>
        /// 重复提交
        /// </summary>
        [Description("重复提交")]
        Repeated = 1009,
        /// <summary>
        /// 余额不足
        /// </summary>
        [Description("余额不足")]
        NoBalance = 1010,
        /// <summary>
        /// 登录失败
        /// </summary>
        [Description("登录失败")]
        LoginFaild = 1011,
        /// <summary>
        /// 金额错误
        /// </summary>
        [Description("金额错误")]
        MoneyInvalid = 1012,
        /// <summary>
        /// 转账参数错误
        /// </summary>
        [Description("转账参数错误")]
        TransferInvalid = 1013,
        /// <summary>
        /// 不支持的币种
        /// </summary>
        [Description("无效的货币")]
        CurrencyInvalid = 1014,
        /// <summary>
        /// 无效的语言代码
        /// </summary>
        [Description("无效的语言代码")]
        LanguageInvalid = 1015,
        /// <summary>
        /// 参数设定错误
        /// </summary>
        [Description("参数设定错误")]
        ConfigInvalid = 1016,
        /// <summary>
        /// 转账订单号已存在
        /// </summary>
        [Description("转账订单号已存在")]
        TransferDuplicate = 1017,
        /// <summary>
        /// 有正在处理的转账订单
        /// </summary>
        [Description("有正在处理的转账订单")]
        TransferProgress = 1018,
        /// <summary>
        /// 无效的交易状态
        /// </summary>
        [Description("无效的交易状态")]
        TransferStatus = 1019,
        /// <summary>
        /// 通常发生于您的 IP 尚未加白，或是 AWC 辨识不到您的 AgentID
        /// </summary>
        [Description("无效的IP")]
        IPInvalid = 1020,
        /// <summary>
        /// 使用无效的装置呼叫(例如：IE)
        /// </summary>
        [Description("无效的客户端")]
        ClientInvalid = 1021,
        [Description("请求超时")]
        Timeout = 1022,
        /// <summary>
        /// HTTP请求错误
        /// </summary>
        [Description("请求错误")]
        RequestInvalid = 1023,

        [Description("游戏关闭")]
        GameClose = 1024,

        [Description("转账失败")]
        TransferFalid = 1025,
        /// <summary>
        /// 游戏代码错误
        /// </summary>
        [Description("游戏代码错误")]
        GameCodeInvalid = 1026,

        [Description("商户锁定")]
        SiteLock = 1027,
        /// <summary>
        /// 商户余额不足
        /// </summary>
        [Description("商户余额不足")]
        SiteNoBalance = 1028,

        /// <summary>
        /// 请求的时间范围错误
        /// </summary>
        [Description("请求时间范围错误")]
        TimeOverflow = 1029,

        /// <summary>
        /// 发生定义之外的错误
        /// </summary>
        [Description("发生错误")]
        Error = 2000,
        /// <summary>
        /// 系统繁忙
        /// </summary>
        [Description("系统繁忙")]
        SystemBuzy = 2001,
        /// <summary>
        /// 不支持该方法
        /// </summary>
        [Description("不支持该方法")]
        NotSupport = 9997,
        /// <summary>
        /// 系统维护中
        /// </summary>
        [Description("系统维护中")]
        Maintenance = 9998,
        /// <summary>
        /// 系统错误
        /// </summary>
        [Description("系统错误")]
        Exception = 9999,
    }
}
