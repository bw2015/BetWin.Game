using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Lottery.Models
{
    /// <summary>
    /// 玩法的赔率设定
    /// 以玩法为单位
    /// </summary>
    public struct Odds
    {

        /// <summary>
        /// 赔率数据（不同的投注内容对应的赔率）
        /// </summary>
        private Dictionary<string, decimal> _data;

        public static implicit operator Odds(Dictionary<string, decimal> data)
        {
            return new Odds { _data = data };
        }

        public static implicit operator Dictionary<string, decimal>(Odds odds)
        {
            return odds._data;
        }

        public void Add(string content, decimal odds)
        {
            if (this._data == null) { this._data = new Dictionary<string, decimal>(); }
            if (this._data.ContainsKey(content))
            {
                this._data[content] = odds;
            }
            else
            {
                this._data.Add(content, odds);
            }
        }

        /// <summary>
        /// 获取赔率
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public decimal GetOdds(string content)
        {
            if (_data.ContainsKey(content)) return _data[content];
            return decimal.Zero;
        }
    }
}
