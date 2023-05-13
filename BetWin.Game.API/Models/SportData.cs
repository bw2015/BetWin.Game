using BetWin.Game.API.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.API.Models
{
    /// <summary>
    /// 体育类型的扩展数据（如果是串关的话就没有）
    /// </summary>
    public class SportData : IOrderData
    {
        [JsonIgnore]
        public string Type => "Sport";

        /// <summary>
        /// 类型（足球/篮球...）
        /// </summary>
        public string sportId { get; set; }

        /// <summary>
        /// 联赛ID
        /// </summary>
        public string leagueId { get; set; }

        /// <summary>
        /// 比赛编号
        /// </summary>
        public string matchId { get; set; }

        /// <summary>
        /// 玩法类型（例如：全场胜者/全场让球/大小球...)
        /// </summary>
        public string playType { get; set; }

        /// <summary>
        /// 下注内容
        /// </summary>
        public string betType { get; set; }

        /// <summary>
        /// 投注项编号
        /// </summary>
        public string betId { get; set; }

        /// <summary>
        /// 下注赔率
        /// </summary>
        public decimal odds { get; set; }

        /// <summary>
        /// 赔率类型（欧赔/香港/马来/印尼/缅甸）
        /// </summary>
        public GameAPIOddsType marketType { get; set; }

        public static implicit operator Dictionary<string, object>(SportData sport)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (sport == null) return data;
            foreach (PropertyInfo property in typeof(SportData).GetProperties())
            {
                data.Add(property.Name, property.GetValue(sport));
            }
            return data;
        }
    }
}
