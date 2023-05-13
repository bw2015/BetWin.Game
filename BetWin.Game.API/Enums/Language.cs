using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Enums
{
    /// <summary>
    /// 语种
    /// </summary>
    public enum Language : byte
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [Description("简体中文"), ISO6391("zh-CN", "zh")]
        CHN = 0,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [Description("正體中文"), ISO6391("zh-TW", "cht")]
        THN = 1,
        /// <summary>
        /// 英文
        /// </summary>
        [Description("English"), ISO6391("en", "en")]
        ENG = 2,
        /// <summary>
        /// 日语
        /// </summary>
        [Description("日本語"), ISO6391("ja", "jp")]
        JA = 3,
        /// <summary>
        /// 韩语
        /// </summary>
        [Description("한국어"), ISO6391("ko", "kor")]
        KO = 4,
        /// <summary>
        /// 越南语
        /// </summary>
        [Description("Tiếng việt"), ISO6391("vi", "vie")]
        VI = 5,
        /// <summary>
        /// 泰语
        /// </summary>
        [Description("ไทย"), ISO6391("th", "th")]
        TH = 6,
        /// <summary>
        /// 西班牙语
        /// </summary>
        [Description("Español"), ISO6391("es", "spa")]
        ES = 7,
        /// <summary>
        /// 葡萄牙语
        /// </summary>
        [Description("Português"), ISO6391("pt", "pt")]
        PT = 8,
        /// <summary>
        /// 法语
        /// </summary>
        [Description("Français"), ISO6391("fr", "fra")]
        FR = 9,
        /// <summary>
        /// 德语
        /// </summary>
        [Description("Deutsch"), ISO6391("de", "de")]
        DE = 10,
        /// <summary>
        /// 意大利语
        /// </summary>
        [Description("Italiano"), ISO6391("it", "it")]
        IT = 11,
        /// <summary>
        /// 俄语
        /// </summary>
        [Description("Русский"), ISO6391("ru", "ru")]
        RU = 12,
        /// <summary>
        /// 印尼语
        /// </summary>
        [Description("indonesia"), ISO6391("id", "id")]
        IND = 13,
        /// <summary>
        /// 丹麦语
        /// </summary>
        [Description("dansk"), ISO6391("da", "dan")]
        DA = 14,
        /// <summary>
        /// 芬兰文
        /// </summary>
        [Description("Suomalainen"), ISO6391("fi", "fin")]
        FI = 15,
        /// <summary>
        /// 荷兰文
        /// </summary>
        [Description("Nederlands"), ISO6391("nl", "nl")]
        NL = 16,
        /// <summary>
        /// 挪威文
        /// </summary>
        [Description("norsk"), ISO6391("no", "nor")]
        NO = 17,
        /// <summary>
        /// 波兰文
        /// </summary>
        [Description("Polski"), ISO6391("pl", "pl")]
        PL = 18,
        /// <summary>
        /// 罗马尼亚文
        /// </summary>
        [Description("rumunjski"), ISO6391("ro", "rom")]
        RO = 19,
        /// <summary>
        /// 瑞典文
        /// </summary>
        [Description("svenska"), ISO6391("sv", "swe")]
        SV = 20,
        /// <summary>
        /// 土耳其文
        /// </summary>
        [Description("Türk"), ISO6391("tr", "tr")]
        TR = 21,
        /// <summary>
        /// 缅甸文
        /// </summary>
        [Description("မြန်မာ"), ISO6391("my", "bur")]
        MY = 22,
        /// <summary>
        /// 阿拉伯语
        /// </summary>
        [Description("العربية"), ISO6391("ar", "ara")]
        AR = 23,
        /// <summary>
        /// 印地语
        /// </summary>
        [Description("हिन्दी"), ISO6391("hi", "hi")]
        HI = 24
    }

    /// <summary>
    /// ISO-639-1 语言代码
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ISO6391Attribute : Attribute
    {
        public ISO6391Attribute(string code, string? baiduCode = null)
        {
            this.Code = code;
            this.BaiduCode = baiduCode;
        }

        /// <summary>
        /// ISO-639-1 语言代码
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// 百度翻译的语言代码
        /// </summary>
        public string? BaiduCode { get; private set; }

        public static implicit operator string(ISO6391Attribute IOS6391)
        {
            return IOS6391.Code;
        }
    }
}
