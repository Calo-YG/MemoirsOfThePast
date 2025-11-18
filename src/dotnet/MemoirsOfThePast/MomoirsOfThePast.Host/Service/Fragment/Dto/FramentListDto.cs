namespace MemoirsOfThePast.HoST.Service.Fragment.Dto
{
    public sealed class FramentListDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 回忆藐视
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 场景
        /// </summary>
        public string Scene { get; set; }
    }
}
