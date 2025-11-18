namespace MemoirsOfThePast.HoST.Service.Memory.Dto
{
    public sealed class MemoryListDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 对话人物名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Background { get; set; }
    }
}
