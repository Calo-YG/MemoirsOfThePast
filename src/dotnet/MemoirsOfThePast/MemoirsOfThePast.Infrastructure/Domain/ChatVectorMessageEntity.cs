namespace MemoirsOfThePast.Infrastructure.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public  class ChatVectorMessageEntity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// AgentTheadId
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// 序列化消息
        /// </summary>
        public string SerializedMessage { get; set; }

        /// <summary>
        /// 消息文本
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AgentId { get; set; }
    }
}
