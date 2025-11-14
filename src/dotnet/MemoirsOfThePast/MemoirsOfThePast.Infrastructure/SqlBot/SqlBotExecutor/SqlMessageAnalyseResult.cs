namespace MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlMessageAnalyseResult
    {
        /// <summary>
        /// 是否进行性能分析
        /// </summary>
        public bool IsAnalyse { get; set; }

        /// <summary>
        /// 是否借助表结构进行性能分析
        /// </summary>
        public bool IsAnalyseUseTable { get; set; }

        /// <summary>
        /// 是否进行错误分析
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 是否借助表结构进行性能分析
        /// </summary>
        public bool IsErrorUseTable { get; set; }

        /// <summary>
        /// 是否生成 sql 语句
        /// </summary>
        public bool IsGenerate { get; set; }
    }
}
