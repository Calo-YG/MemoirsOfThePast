using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;

namespace MemoirsOfThePast.Infrastructure.Executors.Abstract
{
    /// <summary>
    /// 工作流执行上下文
    /// </summary>
    public record class ExecutorContext
    {
        /// <summary>
        /// 执行上下文名称
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// ChatClient
        /// </summary>
        public AIAgent Agent { get; set; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public ILogger logger { get; set; }
    }
}
