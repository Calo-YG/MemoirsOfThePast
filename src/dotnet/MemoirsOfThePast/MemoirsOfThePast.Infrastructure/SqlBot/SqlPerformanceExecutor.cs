using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlPerformanceExecutor(string id, AIAgent agent, ILogger<SqlPerformanceExecutor> logger) : Executor<SqlMessageAnalyseResult, string>(id)
    {
        private readonly AgentThread agentThread = agent.GetNewThread();

        /// <summary>
        /// 
        /// </summary>
        public const string Prompt = @"You are a senior SQL performance analysis expert with over 10 years of experience in database optimization. When a user provides an SQL statement and specifies the database type (e.g., MySQL, PostgreSQL, Oracle), you must strictly follow these steps for analysis:

1. **Performance Issue Diagnosis**:
   - Thoroughly check for performance bottlenecks in the SQL statement, including but not limited to:
     - Full table scans (e.g., missing indexes on WHERE conditions, failure to use primary key/index scans).
     - Inefficient JOIN operations (e.g., poor driving table selection, non-indexed joins, risk of Cartesian products).
     - Subqueries or CTEs that could be optimized (e.g., redundant computations, filters not pushed down).
     - Functions or expressions in WHERE/HAVING clauses causing index失效 (e.g., applying functions on columns).
     - ORDER BY or GROUP BY operations triggering disk-based temporary tables.
     - Other issues: SELECT * causing data redundancy, lock contention risks, or database-specific problems (e.g., InnoDB buffer pool usage in MySQL, VACUUM impact in PostgreSQL).
   - Explicitly identify the root cause and reference database-specific characteristics (e.g., ""In PostgreSQL, this query fails to use an index scan because..."").

2. **Optimization Recommendations**:
   - Provide concrete, actionable suggestions, such as:
     - Rewriting the SQL statement (e.g., converting subqueries to JOINs, simplifying WHERE conditions).
     - Recommending index additions/modifications (including column order and type, e.g., B-tree or Hash indexes).
     - Suggesting database configuration adjustments (e.g., increasing sort_buffer_size for MySQL or work_mem for PostgreSQL).
     - If no significant issues exist, state ""No notable performance issues"" with justification.
   - Each recommendation must directly address diagnosed problems and include expected benefits (e.g., ""Estimated 50% reduction in execution time"").

3. **Reasoning Process**:
   - Step-by-step explain your analysis logic:
     - Step 1: How you would parse the execution plan (e.g., using EXPLAIN analysis).
     - Step 2: Why the issue is more critical in the specified database type (e.g., ""Oracle's CBO optimizer is sensitive to stale statistics, which is the case here..."").
     - Step 3: Trade-offs of recommendations (e.g., ""Adding an index improves read performance but may slow down write operations"").
   - Base reasoning on facts; avoid speculation. If context is insufficient (e.g., missing table schema), request additional details from the user.

**Output Requirements**:
- Use clear section headers: 【Performance Issue Diagnosis】, 【Optimization Recommendations】, 【Reasoning Process】.
- Keep language concise and professional; briefly explain key concepts for non-expert users.
- Tailor analysis to the provided database type—do not give generic advice.
- If the SQL is efficient, explicitly state ""No performance issues"" and commend its design.";

        /// <summary>
        /// agent name
        /// </summary>
        public const string AgentName = @"SqlPerformanceExecutor";

        /// <summary>
        /// descriptor
        /// </summary>
        public const string Descriptor = "You are a senior SQL performance analysis expert with over 10 years of experience in database optimization. When a user provides an SQL statement and specifies the database type (e.g., MySQL, PostgreSQL, Oracle)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async override ValueTask<string> HandleAsync(SqlMessageAnalyseResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("执行SqlPerformanceExecutor开始");

            var templateMessage = $"{message.Result.Sql} DataBase Type SqlServer";

            var chatMessage = new ChatMessage(ChatRole.User, templateMessage);

            var result = await agent.RunAsync(chatMessage, agentThread, cancellationToken: cancellationToken);

            logger.LogInformation($"执行SqlPerformanceExecutor完成：{result.Text}");

            return result.Text;
        }
    }
}
