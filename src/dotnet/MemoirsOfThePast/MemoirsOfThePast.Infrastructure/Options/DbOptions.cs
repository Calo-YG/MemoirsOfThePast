namespace MemoirsOfThePast.Infrastructure.Options
{
    /// <summary>
    /// 
    /// </summary>
    public class DbOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBase { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string SqlType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SyntaxSpec { get; set; }
    }
}
