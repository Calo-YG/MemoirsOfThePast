namespace MemoirsOfThePast.Infrastructure.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public  class ChatHistoryEntity
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 会话id
        /// </summary>
        public string MemoryId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 对话内容
        /// </summary>
        public string Content { get; set; }
    }
}
