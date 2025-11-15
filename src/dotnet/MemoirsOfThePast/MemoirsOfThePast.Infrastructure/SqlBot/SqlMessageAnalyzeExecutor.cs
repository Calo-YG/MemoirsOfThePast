using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotEvent;
using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlMessageAnalyzeExecutor : Executor<string, SqlMessageAnalyseResult>
    {
        /// <summary>
        /// 构建ai agent
        /// </summary>
        private readonly AIAgent aIAgent;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///  
        /// </summary>
        private readonly AgentThread agentThread;


        public const string Prompt = @"You are a SQL semantic analysis assistant. Given a single input text, your task is to determine whether the text contains SQL and classify the user's intent related to SQL. Produce only one JSON object (no additional text, no commentary). Follow these rules exactly.

Output schema (must match exactly):

{
  ""AnalysisSummary"": ""<string, max 2 sentences; high-level, non-sensitive summary; do NOT reveal chain-of-thought>"",
  ""Result"": {
    ""IsAnalyse"": <boolean>,
    ""IsAnalyseUseTable"": <boolean>,
    ""IsError"": <boolean>,
    ""IsErrorUseTable"": <boolean>,
    ""IsGenerate"": <boolean>,
    ""Sql"": ""<string (SQL statement) or empty string>"",
    ""SqlType"": ""<string (e.g., \""MySQL\"", \""PostgreSQL\"", \""SQLServer\"", \""Oracle\"") or empty string>""
  }
}


Deterministic rules (apply in order):

Only rely on the provided input text. Do not call external services or assume unstated context.

IsAnalyse = true if the explicit intent of the input is to analyze an SQL statement (e.g., “analyze”, “explain”, “optimize”, “classify” when clearly targeted at SQL). Otherwise false.

IsAnalyseUseTable = true if the input mentions table structures, schemas, columns, indexes, or DDL in the context of analysis; otherwise false.

IsError = true if the input explicitly asks to find, fix, or explain errors/bugs in an SQL statement (e.g., “fix this error”, “why does this query fail”); otherwise false.

IsErrorUseTable = true if the error-related request refers to table/schema/column definitions or provides schema details relevant to diagnosing the error; otherwise false.

IsGenerate = true if the input requests creation of new SQL (e.g., “write”, “generate”, “create a query”) or templates; otherwise false.

Sql: if the input contains one or more SQL statements, extract the most complete contiguous SQL statement (preserve original formatting but trim leading/trailing whitespace); if none, set to empty string. If multiple statements appear and intent targets all statements, concatenate them with a single semicolon and a space (""; "") between statements.

SqlType: infer the likely DB engine when the SQL contains engine-specific syntax (e.g., LIMIT without OFFSET often → MySQL/PostgreSQL ambiguous; TOP → SQLServer; RETURNING → PostgreSQL; PL/SQL blocks → Oracle; IDENTITY → SQLServer; AUTO_INCREMENT → MySQL). If inference is uncertain, return an empty string. Do not guess beyond reasonable syntax signals.

Ambiguity fallback: if the overall intent is ambiguous (no clear analysis/error/generation intent), set IsAnalyse=false, IsError=false, IsGenerate=false, leave Sql and SqlType empty unless a clear SQL snippet is present.

Do not include any keys other than the seven shown. Types must match exactly (booleans for the flags, strings for Sql and SqlType). AnalysisSummary must be ≤ 2 sentences and must not include chain-of-thought, internal deliberation, or stepwise reasoning.

Examples (for implementer clarity only — do NOT output examples in actual responses):

Input: ""请帮我优化：SELECT * FROM users WHERE id = ?"" → IsAnalyse=true, Sql=""SELECT * FROM users WHERE id = ?"", SqlType="""" (unless specific DB indicators present).

Input: ""Why does SELECT TOP 10 * FROM t fail?"" → IsError=true, Sql=""SELECT TOP 10 * FROM t"", SqlType=""SQLServer"".

Strictly produce only the JSON object following the schema above when given an input text.
";

        /// <summary>
        /// agent name
        /// </summary>
        public const string AgentName = @"sqlanalyse";

        /// <summary>
        ///  agent descriptor 
        /// </summary>
        public const string Descriptor = "You are a SQL semantic analysis assistant. Your task is to analyze a user-provided text and classify their intent related to SQL. You must return a structured JSON output only, without any explanations or additional commentary.";

        public SqlMessageAnalyzeExecutor(string id, AIAgent aIAgent, ILogger<SqlMessageAnalyzeExecutor> logger):base(id) 
        { 
            this.aIAgent = aIAgent;
            this._logger = logger;
            agentThread = aIAgent.GetNewThread();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<SqlMessageAnalyseResult> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Receive UserInput:{message}");

            var result = await this.aIAgent.RunAsync(message, this.agentThread, cancellationToken: cancellationToken);

            var sloganResult = JsonSerializer.Deserialize<SqlMessageAnalyseResult>(result.Text) ?? throw new InvalidOperationException("Failed to deserialize SqlMessageAnalyseResult result.");

            _logger.LogInformation($"Agent Output:{result.Text}");

            await context.AddEventAsync(new SqlMessageAnalyseEvent(sloganResult), cancellationToken);

            return sloganResult;
        }
    }
}
