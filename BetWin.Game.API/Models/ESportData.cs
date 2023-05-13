using BetWin.Game.API.Enums;

namespace BetWin.Game.API.Models
{
    public class ESportData : IOrderData
    {
        public string Type => "ESport";

        /// <summary>
        /// 游戏id
        /// </summary>
        public string gameId { get; set; }
        /// <summary>
        /// 联赛id
        /// </summary>
        public string leagueId { get; set; }

        /// <summary>
        /// 赛事id
        /// </summary>
        public string matchId { get; set; }

        /// <summary>
        /// 盘口ID
        /// </summary>
        public string betId { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        public decimal odds { get; set; }

        /// <summary>
        /// 投注内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 赔率类型（欧赔/香港/马来/印尼/缅甸）
        /// </summary>
        public GameAPIOddsType oddsType { get; set; }

    }
}
