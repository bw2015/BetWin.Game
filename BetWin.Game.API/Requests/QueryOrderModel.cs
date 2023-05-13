namespace BetWin.Game.API.Requests
{
    public class QueryOrderModel : GameModelBase
    {
        public QueryOrderModel() : base(0)
        {
        }

        /// <summary>
        /// 任务索引（自增）
        /// </summary>
        public int TaskIndex { get; set; }

        /// <summary>
        /// 线路下分类代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 开始时间(13位时间戳)
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 结束时间(13位时间戳)
        /// </summary>
        public long EndTime { get; set; }
    }
}
