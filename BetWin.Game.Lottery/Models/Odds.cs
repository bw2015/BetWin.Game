﻿using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static implicit operator string[](Odds odds)
        {
            return odds._data.Keys.ToArray();
        }

        /// <summary>
        /// 设定赔率
        /// </summary>
        /// <param name="content"></param>
        /// <param name="odds"></param>
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

        public override string ToString()
        {
            return this.ToString(2000);
        }

        /// <summary>
        /// 根据用户的发点返回玩法赔率
        /// </summary>
        /// <param name="rebate">最大值2000</param>
        /// <param name="numbers">自定义的号码</param>
        /// <returns></returns>
        public string ToString(int rebate, string[]? numbers = null)
        {
            rebate = Math.Min(2000, rebate);



            StringBuilder sb = new StringBuilder("{");
            if (numbers == null)
            {
                sb.Append(string.Join(",", this._data.Select(t => $"\"{t.Key}\":{Math.Floor(t.Value * (decimal)rebate / 2000M * 100M) / 100M}")));
            }
            else
            {
                var data = this._data;
                sb.Append(string.Join(",", numbers.Select(t =>
                {
                    return string.Concat($"\"{t}\":",
                        (data.ContainsKey(t) ? Math.Floor(data[t] * (decimal)rebate / 2000M * 100M) / 100M : 0M));
                })));
            }
            sb.Append("}");
            return sb.ToString();
        }

        internal bool Contains(string content)
        {
            return this._data.ContainsKey(content);
        }

        public static Odds operator +(Odds odds1, Odds odds2)
        {
            foreach (string key in odds2._data.Keys)
            {
                odds1.Add(key, odds2._data[key]);
            }
            return odds1;
        }

        /// <summary>
        /// 按照赔率的大小进行新赔率匹配（适用于非固定赔率的场景）
        /// </summary>
        /// <param name="indexOdds"></param>
        public void UpdateByIndex(Dictionary<string, decimal> indexOdds)
        {
            decimal[] oddsValues = this._data.Values.OrderBy(t => t).ToArray();
            decimal[] indexOddsValues = indexOdds.Values.OrderBy(t => t).ToArray();

            foreach (string number in indexOdds.Keys.ToArray())
            {
                int indexValue = Array.IndexOf(indexOddsValues, indexOdds[number]);
                if (indexValue == -1) continue;
                this._data[number] = oddsValues[indexValue];
            }
        }
    }
}
