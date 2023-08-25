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
    public struct StepTimeModel
    {
        public StepTimeModel(long startTime, long endTime, string? index = null)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.index = index;
        }

        /// <summary>
        /// 当局的开始时间
        /// </summary>
        public long startTime;

        /// <summary>
        /// 当局的结束时间
        /// </summary>
        public long endTime;

        /// <summary>
        /// 期号（不一定有）
        /// </summary>
        public string? index;

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
