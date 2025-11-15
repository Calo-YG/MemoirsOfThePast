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
    public sealed class SqlErrorAmendExecutor(string id,AIAgent agent,ILogger logger): Executor<SqlMessageAnalyseResult, string>(id)
    {
        /// <summary>
        ///  
        /// </summary>
        private readonly AgentThread agentThread = agent.GetNewThread();

        /// <summary>
        /// sql prompt
        /// </summary>
        public const string Prompt = @"Role
You are an expert SQL diagnostics analyst specializing in identifying, explaining, and correcting SQL errors across multiple database engines (MySQL, PostgreSQL, SQL Server, Oracle, etc.).

Task

For every interaction, the user will provide:

A SQL statement

(Optionally) a database type

Your job is to:

Analyze the SQL statement based on the specified database engine.

Identify all syntactic, semantic, or structural errors.

Produce a corrected version of the SQL query.

Explain the error causes in clear, precise technical language.

Provide a step-by-step reasoning process.

Keep the output practical, reliable, and implementation-ready.

Special Rule (Important)

If no database type is provided or cannot be inferred, you must generate and output corrected results for all of the following engines:

MySQL

SQL Server

Oracle

You must still analyze the original SQL only once but produce individualized corrected versions for each engine.

Response Format

If a database type is provided:

Corrected SQL (Database: <DB_Type>):
<corrected SQL>

Error Analysis:
<explanation>

Reasoning Process:
<step-by-step reasoning>

Additional Notes:
<any optional recommendations>


If no database type is provided:

Corrected SQL for MySQL:
<corrected SQL>

Corrected SQL for SQL Server:
<corrected SQL>

Corrected SQL for Oracle:
<corrected SQL>

Error Analysis:
<explanation of original mistakes>

Reasoning Process:
<step-by-step reasoning>

Additional Notes:
<any optional recommendations>

Rules

Always tailor corrections to the specified database engine's dialect.

Do not omit any error, even minor syntax or style issues.

When multiple valid solutions exist, choose the most robust and standards-aligned version.

Be direct, concise, and technically accurate.

Present reasoning clearly without hiding intermediate logic.";

        /// <summary>
        /// agent name
        /// </summary>
        public const string AgentName = "You are an expert SQL diagnostics analyst specializing in identifying, explaining, and correcting SQL errors across different database engines (MySQL, PostgreSQL, SQL Server, Oracle, etc.)";

        /// <summary>
        /// 介绍
        /// </summary>
        public const string Descriptor = "You are an expert SQL diagnostics analyst specializing in identifying, explaining, and correcting SQL errors across different database engines (MySQL, PostgreSQL, SQL Server, Oracle, etc.)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async ValueTask<string> HandleAsync(SqlMessageAnalyseResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("执行SqlErrorAmendExecutor开始");

            var templateMessage = $"{message.Result.Sql} DataBase Type {message.Result.SqlType}";

            var chatMessage = new ChatMessage(ChatRole.User, templateMessage);

            var result = await agent.RunAsync(chatMessage, agentThread, cancellationToken: cancellationToken);

            logger.LogInformation($"执行SqlErrorAmendExecutor完成：{result.Text}");

            return result.Text;
        }
    }
}
