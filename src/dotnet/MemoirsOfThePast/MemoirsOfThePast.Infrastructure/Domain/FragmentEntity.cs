namespace MemoirsOfThePast.Infrastructure.Domain
{
    /// <summary>
    /// 回忆碎片
    /// </summary>
    public class FragmentEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 回忆id
        /// </summary>
        public string MemoryId { get; set; }

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

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
