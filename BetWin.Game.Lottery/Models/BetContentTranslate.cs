using BetWin.Game.Lottery.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Models
{
    /// <summary>
    /// 投注内容的多语种翻译
    /// </summary>
    public struct BetContentTranslate
    {
        /// <summary>
        /// 投注内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 多语种内容
        /// </summary>
        public Dictionary<Language, string> languages { get; set; }

        public BetContentTranslate(string content)
        {
            this.Content = content;
            languages = new Dictionary<Language, string>();
        }

        public BetContentTranslate(string content, string chn, string eng, string? vi = null, string? th = null, string? ind = null, string? hi = null)
        {
            this.Content = content;
            this.languages = new Dictionary<Language, string>()
            {
                {Language.CHN,chn },
                {Language.ENG,eng },
                {Language.VI,vi??eng },
                {Language.TH,th??eng },
                {Language.IND,ind??eng },
                {Language.HI,hi??eng },
            };
        }

        public string Get(Language language)
        {
            if (this.languages.ContainsKey(language)) return this.languages[language];
            if (this.languages.ContainsKey(Language.ENG)) return this.languages[Language.ENG];
            return this.Content;
        }
    }
}
