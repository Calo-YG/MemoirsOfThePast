namespace MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlMessageAnalyseResult
    {
        /// <summary>
        /// 思考过程
        /// </summary>
        public string AnalysisSummary { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public MessageAnalyzeResult Result { get; set; }
    }

    public sealed class MessageAnalyzeResult
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

        /// <summary>
        /// 用户message
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string SqlType { get; set; }

        /// <summary>
        /// 查询所用的表
        /// </summary>
        public List<string> Tables { get; set; }

        /// <summary>
        /// 列信息
        /// </summary>
        public List<string> Columns { get; set; }
    }

    /// <summary>
    /// 列信息
    /// </summary>
    public sealed class ColumnInfo
    {
        /// <summary>
        /// 表格信息
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
    }
}
