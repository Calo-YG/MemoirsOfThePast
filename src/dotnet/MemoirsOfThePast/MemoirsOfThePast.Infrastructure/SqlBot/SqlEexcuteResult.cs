namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlEexcuteResult
    {
        /// <summary>
        /// 是否分析
        /// </summary>
        public bool IsAnalyse { get; set;  }

        /// <summary>
        /// 是否是查询
        /// </summary>
        public bool IsQuery { get; set; }

        /// <summary>
        /// 数据结果
        /// </summary>
        public string Data { get; set; }
    }
}
