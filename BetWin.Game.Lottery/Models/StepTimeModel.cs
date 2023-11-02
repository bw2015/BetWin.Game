using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BetWin.Game.Lottery.Models
{
    /// <summary>
    /// 局局乐的开奖时间
    /// </summary>
    public class StepTimeModel
    {
        public StepTimeModel(string index, long openTime, long startTime, long? endTime = null)
        {
            this.index = index;
            this.openTime = openTime;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        /// <summary>
        /// 期号
        /// </summary>
        public string index { get; set; }

        /// <summary>
        /// 当前期的开奖时间
        /// </summary>
        public long openTime { get; set; }

        /// <summary>
        /// 当局的开始时间（可投注时间）
        /// </summary>
        public long startTime { get; set; }

        /// <summary>
        /// 当局的封盘时间（如果不设置的话，则使用开奖时间减去封单时间）
        /// </summary>
        public long? endTime { get; set; }


        public static implicit operator bool(StepTimeModel stepTime)
        {
            return stepTime.startTime != 0 && stepTime.endTime != 0;
        }

        public static implicit operator StepTimeModel(string value)
        {
            if (string.IsNullOrEmpty(value)) return default;
            return JsonConvert.DeserializeObject<StepTimeModel>(value);
        }
    }
}
