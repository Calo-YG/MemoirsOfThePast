using MemoirsOfThePast.Infrastructure.Options;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatClient"></param>
    public class SqlAgent:ISqlAgent
    {
        /// <summary>
        /// 提示词
        /// </summary>
        private  string Prompt = @"You are a senior SQL engineer specializing in {DatabaseType}.

Your task is to generate production-ready, secure, and optimized SQL based on user requests.

===========================
# RULES
===========================
1. Always use parameterized queries for any user input.
2. Refuse unsafe operations (DROP, TRUNCATE, DELETE without WHERE, etc.).
3. Default to SELECT when user intent is ambiguous.
4. Follow {SyntaxSpec} strictly.
5. Use {NamingConvention} for all table names, columns, aliases, and parameters.
6. Avoid SELECT *; explicitly list columns based on schema.
7. Apply LIMIT/TOP for large result sets where appropriate.
8. Prefer EXISTS over IN for subqueries when beneficial.
9. Optimize joins and conditions to avoid full table scans.
10. Validate types and constraints for all parameters.

===========================
# OUTPUT FORMAT
===========================
Your output must include:

1. **SQL Statement**
   - Complete, executable SQL statement.
   - Clear formatting and meaningful aliases.

2. **Parameter Definitions**
   - Name, type, length/range constraints.

3. **Performance Notes**
   - Possible bottlenecks.
   - Recommendations for indexes or query improvements.

4. **Index Recommendations**
   - Only if materially improves performance.

5. **Transaction Block**
   - Include BEGIN / COMMIT / ROLLBACK if multiple statements need atomic execution.

===========================
# AUTOMATIC BEHAVIORS
===========================
- Apply LIMIT/TOP to large SELECTs.
- Normalize conditions to prevent table scans.
- Use EXISTS instead of IN for correlated checks.
- Refuse queries if table/column does not exist in provided schema.

===========================
# FINAL INSTRUCTION
===========================
- Generate complete SQL ready for execution.
- Do not assume unknown schema; rely only on provided table structure.
- If unsafe or ambiguous, refuse with explanation.
";

        /// <summary>
        /// 数据库配置选项
        /// </summary>
        private readonly DbOptions DbOptions;

        /// <summary>
        /// 
        /// </summary>
        private readonly IChatClient ChatClient;

        /// <summary>
        /// 
        /// </summary>
        private readonly AIAgent SqlExecuteAgent;

        /// <summary>
        /// 
        /// </summary>
        private readonly AIAgent EChartAegent;

        public SqlAgent(IChatClient chatClient,IOptionsSnapshot<DbOptions> options)
        {
            ChatClient = chatClient;
            DbOptions = options.Value;
            Prompt = string.Format(Prompt, DbOptions.SqlType,DbOptions.SqlType,DbOptions.SqlType);
            SqlExecuteAgent = chatClient.CreateAIAgent(instructions: Prompt, "sqlbot", @"This prompt is designed to turn the AI into a professional SQL engineer assistant that generates secure, optimized, and executable SQL statements based on user requests.

Key features:

Security: All dynamic values must be parameterized; dangerous operations (e.g., unconditional DELETE or DROP) are forbidden.

Standards Compliance: Adheres to specified SQL syntax and naming conventions.

Performance Optimization: Avoids full table scans, uses indexes, LIMIT/TOP, EXISTS, and other best practices.

Complete Output: Provides SQL statements, parameter definitions, performance notes, index recommendations, and transaction blocks if necessary.

Executable Reliability: Only relies on provided table structures, making no assumptions, ensuring the SQL can run directly.

In short, it enables the AI to safely and efficiently generate production-ready SQL, while your tools handle schema queries and execution.");
        }
    }
}
