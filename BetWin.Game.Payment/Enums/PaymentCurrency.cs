﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.Payment.Enums
{
    /// <summary>
    /// 支付接口的币种
    /// </summary>
    public enum PaymentCurrency
    {
        [Description("人民币")]
        CNY = 0,
        [Description("美元")]
        USD = 1,
        [Description("新台币")]
        TWD = 2,
        [Description("欧元")]
        EUR = 3,
        [Description("泰铢")]
        THB = 4,
        [Description("越南盾")]
        VND = 5,
        [Description("印尼盾")]
        IDR = 6,
        [Description("菲律宾比索")]
        PHP = 7,
        [Description("俄罗斯卢布")]
        RUB = 8,
        [Description("日元")]
        JPY = 9,
        /// <summary>
        /// 韩元
        /// </summary>
        [Description("韩元")]
        KRW = 10,
        [Description("印度卢比")]
        INR = 11,
        /// <summary>
        /// 马来西亚林吉特
        /// </summary>
        [Description("马来西亚林吉特")]
        MYR = 12,
        [Description("千越南盾")]
        KVND = 13,
        /// <summary>
        /// 千印尼盾
        /// </summary>
        [Description("千印尼盾")]
        KIDR = 14,
        [Description("金币")]
        COIN = 15,
        [Description("港币")]
        HKD = 16,
        [Description("USDT")]
        USDT = 17,
        /// <summary>
        /// 新加坡元
        /// </summary>
        [Description("新加坡元")]
        SGD = 18,
        /// <summary>
        /// 英镑
        /// </summary>
        [Description("英镑")]
        GBP = 19,
        /// <summary>
        /// 比特币
        /// </summary>
        [Description("比特币")]
        BTC = 20,
        /// <summary>
        /// 澳元
        /// </summary>
        [Description("澳元")]
        AUD = 21,
        /// <summary>
        /// 加拿大元
        /// </summary>
        [Description("加拿大元")]
        CAD = 22,
        /// <summary>
        /// 埃及镑
        /// </summary>
        [Description("埃及镑")]
        EGP = 23,
        /// <summary>
        /// 阿曼里亚尔
        /// </summary>
        [Description("阿曼里亚尔")]
        OMR = 24,
        /// <summary>
        /// 阿尔及利亚第纳尔
        /// </summary>
        [Description("阿尔及利亚第纳尔")]
        DZD = 25,
        /// <summary>
        /// 澳门元
        /// </summary>
        [Description("澳门元")]
        MOP = 26,
        /// <summary>
        /// 阿联酋迪拉姆
        /// </summary>
        [Description("阿联酋迪拉姆")]
        AED = 27,
        /// <summary>
        /// 孟加拉国塔卡
        /// </summary>
        [Description("孟加拉国塔卡")]
        BDT = 28,
        /// <summary>
        /// 汶莱元
        /// </summary>
        [Description("汶莱元")]
        BND = 29,
        /// <summary>
        /// 巴西雷亚尔
        /// </summary>
        [Description("巴西雷亚尔")]
        BRL = 30,
        /// <summary>
        /// 瑞士法郎
        /// </summary>
        [Description("瑞士法郎")]
        CHF = 31,
        /// <summary>
        /// 哥伦比亚比索
        /// </summary>
        [Description("哥伦比亚比索")]
        COP = 32,
        /// <summary>
        /// 哈萨克坦吉
        /// </summary>
        [Description("哈萨克坦吉")]
        KZT = 33,
        /// <summary>
        /// 千柬埔寨瑞尔
        /// </summary>
        [Description("千柬埔寨瑞尔")]
        KKHR = 34,
        /// <summary>
        /// 柬埔寨老挝基普
        /// </summary>
        [Description("柬埔寨老挝基普")]
        LAK = 35,
        /// <summary>
        /// 千柬埔寨老挝基普
        /// </summary>
        [Description("千柬埔寨老挝基普")]
        KLAK = 36,
        /// <summary>
        /// 斯里兰卡卢比
        /// </summary>
        [Description("斯里兰卡卢比")]
        LKR = 37,
        /// <summary>
        /// 缅甸元
        /// </summary>
        [Description("缅甸元")]
        MMK = 38,
        /// <summary>
        /// 千缅甸元
        /// </summary>
        [Description("千缅甸元")]
        KMMK = 39,
        /// <summary>
        /// 墨西哥比索
        /// </summary>
        [Description("墨西哥比索")]
        MXN = 40,
        /// <summary>
        /// 蒙古图格里克
        /// </summary>
        [Description("蒙古图格里克")]
        MNT = 41,
        /// <summary>
        /// 挪威克朗
        /// </summary>
        [Description("挪威克朗")]
        NOK = 42,
        /// <summary>
        /// 新西兰元
        /// </summary>
        [Description("新西兰元")]
        NZD = 43,
        /// <summary>
        /// 尼泊尔卢比
        /// </summary>
        [Description("尼泊尔卢比")]
        NPR = 44,
        /// <summary>
        /// 奈及利亚奈拉
        /// </summary>
        [Description("奈及利亚奈拉")]
        NGN = 45,
        /// <summary>
        /// 巴基斯坦卢比
        /// </summary>
        [Description("巴基斯坦卢比")]
        PKR = 46,
        /// <summary>
        /// 秘鲁新索尔
        /// </summary>
        [Description("秘鲁新索尔")]
        PEN = 47,
        /// <summary>
        /// 瑞典克朗
        /// </summary>
        [Description("瑞典克朗")]
        SEK = 48,
        /// <summary>
        /// 土耳其里拉
        /// </summary>
        [Description("土耳其里拉")]
        TRY = 49,
        /// <summary>
        /// 突尼斯第纳尔
        /// </summary>
        [Description("突尼斯第纳尔")]
        TND = 50,
        /// <summary>
        /// 乌克兰赫夫纳
        /// </summary>
        [Description("乌克兰赫夫纳")]
        UAH = 51,
        /// <summary>
        /// 南非兰特
        /// </summary>
        [Description("南非兰特")]
        ZAR = 52,
        /// <summary>
        /// 津巴布韦元
        /// </summary>
        [Description("津巴布韦元")]
        ZWD = 53,
        /// <summary>
        /// 阿富汗
        /// </summary>
        [Description("阿富汗")]
        AFN = 54,
        /// <summary>
        /// 阿尔巴尼亚列克
        /// </summary>
        [Description("阿尔巴尼亚列克")]
        ALL = 55,
        /// <summary>
        /// 亚美尼亚德拉姆
        /// </summary>
        [Description("亚美尼亚德拉姆")]
        AMD = 56,
        /// <summary>
        /// 安的列斯盾
        /// </summary>
        [Description("安的列斯盾")]
        ANG = 57,
        /// <summary>
        /// 安哥拉宽扎
        /// </summary>
        [Description("安哥拉宽扎")]
        AOA = 58,
        /// <summary>
        /// 阿根廷比索
        /// </summary>
        [Description("阿根廷比索")]
        ARS = 59,
        /// <summary>
        /// 阿鲁巴弗罗林
        /// </summary>
        [Description("阿鲁巴弗罗林")]
        AWG = 60,
        /// <summary>
        /// 阿塞拜疆马纳特
        /// </summary>
        [Description("阿塞拜疆马纳特")]
        AZN = 61,
        /// <summary>
        /// 波黑可兑换马克
        /// </summary>
        [Description("波黑可兑换马克")]
        BAM = 62,
        /// <summary>
        /// 巴巴多斯元
        /// </summary>
        [Description("巴巴多斯元")]
        BBD = 63,
        /// <summary>
        /// 巴林第纳尔
        /// </summary>
        [Description("巴林第纳尔")]
        BHD = 64,
        /// <summary>
        /// 布隆迪法郎
        /// </summary>
        [Description("布隆迪法郎")]
        BIF = 65,
        /// <summary>
        /// 千布隆迪法郎
        /// </summary>
        [Description("千布隆迪法郎")]
        KBIF = 66,
        [Description("保加利亚列弗")]
        BGN = 67,
        [Description("玻利维亚诺")]
        BOB = 68,
        [Description("巴哈马元")]
        BSD = 69,
        [Description("不丹努尔特鲁姆")]
        BTN = 70,
        [Description("博茨瓦纳普拉")]
        BWP = 71,
        [Description("白俄罗斯卢布")]
        BYN = 72,
        [Description("伯利兹元")]
        BZD = 73,
        [Description("刚果法郎")]
        CDF = 74,
        [Description("刚果法郎")]
        KCDF = 75,
        [Description("智利比索")]
        CLP = 76,
        [Description("哥斯达黎加科朗")]
        CRC = 77,
        [Description("千哥伦比亚比索")]
        KCRC = 78,
        [Description("塞尔维亚第纳尔")]
        CSD = 79,
        [Description("古巴比索")]
        CUP = 80,
        [Description("佛得角埃斯库多")]
        CVE = 81,
        [Description("捷克克朗")]
        CZK = 82,
        [Description("吉布提法郎")]
        DJF = 83,
        [Description("丹麦克朗")]
        DKK = 84,
        [Description("多米尼加比索")]
        DOP = 85,
        [Description("厄立特里亚纳克法")]
        ERN = 86,
        [Description("埃塞俄比亚比尔")]
        ETB = 87,
        [Description("斐济元")]
        FJD = 88,
        [Description("格鲁吉亚拉里")]
        GEL = 89,
        [Description("加纳塞地")]
        GHS = 90,
        [Description("直布罗陀庞德")]
        GIP = 91,
        [Description("冈比亚货币")]
        GMD = 92,
        /// <summary>
        /// 几内亚法郎
        /// </summary>
        [Description("几内亚法郎")]
        GNF = 93,
        /// <summary>
        /// 千几内亚法郎
        /// </summary>
        [Description("千几内亚法郎")]
        KGNF = 94,
        [Description("危地马拉格查尔")]
        GTQ = 95,
        [Description("圭亚那元")]
        GYD = 96,
        [Description("洪都拉斯伦皮拉")]
        HNL = 97,
        [Description("克罗地亚库纳")]
        HRK = 98,
        [Description("海地古德")]
        HTG = 99,
        [Description("匈牙利福林")]
        HUF = 100,
        /// <summary>
        /// 伊拉克第纳尔
        /// </summary>
        [Description("伊拉克第纳尔")]
        IQD = 101,
        /// <summary>
        /// 千伊拉克第纳尔
        /// </summary>
        [Description("千伊拉克第纳尔")]
        KIQD = 102,
        /// <summary>
        /// 伊朗里亚尔
        /// </summary>
        [Description("伊朗里亚尔")]
        IRR = 103,
        /// <summary>
        /// 千伊朗里亚尔
        /// </summary>
        [Description("千伊朗里亚尔")]
        KIRR = 104,
        [Description("冰岛克朗")]
        ISK = 105,
        [Description("牙买加元")]
        JMD = 106,
        [Description("约旦第纳尔")]
        JOD = 107,
        [Description("肯尼亚先令")]
        KES = 108,
        [Description("吉尔吉斯斯坦索姆")]
        KGS = 109,
        [Description("柬埔寨利尔斯")]
        KHR = 110,
        [Description("科摩罗法郎")]
        KMF = 111,
        [Description("北朝鲜元")]
        KPW = 112,
        /// <summary>
        /// 千韩元
        /// </summary>
        [Description("千韩元")]
        KKRW = 113,
        [Description("科威特第纳尔")]
        KWD = 114,
        [Description("开曼岛元")]
        KYD = 115,
        /// <summary>
        /// 黎巴嫩镑
        /// </summary>
        [Description("黎巴嫩镑")]
        LBP = 116,
        /// <summary>
        /// 千黎巴嫩镑
        /// </summary>
        [Description("千黎巴嫩镑")]
        KLBP = 117,
        [Description("黎巴嫩元")]
        LRD = 118,
        [Description("莱索托洛蒂")]
        LSL = 119,
        [Description("拉脱维亚拉特")]
        LVL = 120,
        [Description("利比亚第纳尔")]
        LYD = 121,
        [Description("摩洛哥迪拉姆")]
        MAD = 122,
        [Description("摩尔多瓦列伊")]
        MDL = 123,
        /// <summary>
        /// 马达加斯加阿里亚里
        /// </summary>
        [Description("马达加斯加阿里亚里")]
        MGA = 124,
        /// <summary>
        /// 千马达加斯加阿里亚里
        /// </summary>
        [Description("千马达加斯加阿里亚里")]
        KMGA = 125,
        [Description("马其顿第纳尔")]
        MKD = 126,
        [Description("毛里求斯卢比")]
        MUR = 127,
        [Description("马尔代夫罗非亚")]
        MVR = 128,
        [Description("马拉维克瓦查")]
        MWK = 129,
        [Description("莫桑比克梅蒂卡尔")]
        MZN = 130,
        [Description("纳米比亚元")]
        NAD = 131,
        [Description("尼加拉瓜科多巴")]
        NIO = 132,
        [Description("巴拿马巴波亚")]
        PAB = 133,
        [Description("巴布亚新几内亚基纳")]
        PGK = 134,
        [Description("波兰兹罗提")]
        PLN = 135,
        [Description("卡塔尔里亚尔")]
        QAR = 136,
        /// <summary>
        /// 巴拉圭瓜拉尼
        /// </summary>
        [Description("巴拉圭瓜拉尼")]
        PYG = 137,
        /// <summary>
        /// 千巴拉圭瓜拉尼
        /// </summary>
        [Description("千巴拉圭瓜拉尼")]
        KPYG = 138,
        /// <summary>
        /// 罗马尼亚列伊
        /// </summary>
        [Description("罗马尼亚列伊")]
        RON = 139,
        /// <summary>
        /// 塞尔维亚第纳尔
        /// </summary>
        [Description("塞尔维亚第纳尔")]
        RSD = 140,
        /// <summary>
        /// 卢旺达法郎
        /// </summary>
        [Description("卢旺达法郎")]
        RWF = 141,
        /// <summary>
        /// 千卢旺达法郎
        /// </summary>
        [Description("千卢旺达法郎")]
        KRWF = 142,
        [Description("沙特里亚尔")]
        SAR = 143,
        [Description("所罗门群岛元")]
        SBD = 144,
        [Description("塞舌尔卢比")]
        SCR = 145,
        [Description("苏丹镑")]
        SDG = 146,
        [Description("圣赫勒拿磅")]
        SHP = 147,
        /// <summary>
        /// 塞拉利昂利昂
        /// </summary>
        [Description("塞拉利昂利昂")]
        SLL = 148,
        /// <summary>
        /// 千塞拉利昂利昂
        /// </summary>
        [Description("千塞拉利昂利昂")]
        KSLL = 149,
        [Description("索马里先令")]
        SOS = 150,
        [Description("苏里南元")]
        SRD = 151,
        [Description("萨尔瓦多科朗")]
        SVC = 152,
        [Description("叙利亚镑")]
        SYP = 153,
        [Description("塔吉克斯坦索莫尼")]
        TJS = 154,
        [Description("土库曼斯坦新马纳特")]
        TMT = 155,
        [Description("汤加潘加")]
        TOP = 156,
        [Description("特立尼达与多巴哥元")]
        TTD = 157,
        [Description("TUSD")]
        TUSD = 158,
        /// <summary>
        /// 坦桑尼亚先令
        /// </summary>
        [Description("坦桑尼亚先令")]
        TZS = 159,
        /// <summary>
        /// 千坦桑尼亚先令
        /// </summary>
        [Description("千坦桑尼亚先令")]
        KTZS = 160,
        [Description("微比特币")]
        UBTC = 161,
        [Description("乌干达先令")]
        UGX = 162,
        [Description("千乌干达先令")]
        KUGX = 163,
        [Description("USDC")]
        USDC = 164,
        [Description("乌拉圭比索")]
        UYU = 165,
        /// <summary>
        /// 乌兹别克斯坦苏姆
        /// </summary>
        [Description("乌兹别克斯坦苏姆")]
        UZS = 166,
        /// <summary>
        /// 千乌兹别克斯坦苏姆
        /// </summary>
        [Description("千乌兹别克斯坦苏姆")]
        KUZS = 167,
        [Description("瓦努阿图瓦图")]
        VUV = 168,
        [Description("萨摩亚塔拉")]
        WST = 169,
        /// <summary>
        /// 中非金融合作法郎
        /// </summary>
        [Description("中非金融合作法郎")]
        XAF = 170,
        /// <summary>
        /// 东加勒比元
        /// </summary>
        [Description("东加勒比元")]
        XCD = 171,
        /// <summary>
        /// CFA法郎
        /// </summary>
        [Description("CFA法郎")]
        XOF = 172,
        /// <summary>
        /// 太平洋法郎
        /// </summary>
        [Description("太平洋法郎")]
        XPF = 173,
        /// <summary>
        /// 也门里亚尔
        /// </summary>
        [Description("也门里亚尔")]
        YER = 174,
        /// <summary>
        /// 赞比亚克瓦查
        /// </summary>
        [Description("赞比亚克瓦查")]
        ZMW = 175,
        /// <summary>
        /// 千哥伦比亚比索
        /// </summary>
        [Description("千哥伦比亚比索")]
        KCOP = 176,
        /// <summary>
        /// 百慕大元
        /// </summary>
        [Description("百慕大元")]
        BMD = 177,
        /// <summary>
        /// 千蒙古图格里克
        /// </summary>
        [Description("千蒙古图格里克")]
        KMNT = 178,
    }
}
