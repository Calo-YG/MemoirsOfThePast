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


        public const string Prompt = @"You are a SQL semantic analysis assistant. Your job is to analyze a single input text and classify the user’s SQL-related intent, while extracting the tables and columns involved.  
Your output must be only one JSON object, with no extra text or commentary.

Output schema (must match exactly):

{
  ""AnalysisSummary"": ""<string, max 2 sentences; high-level, non-sensitive summary; do NOT reveal chain-of-thought>"",
  ""Result"": {
    ""IsAnalyse"": <boolean>,
    ""IsAnalyseUseTable"": <boolean>,
    ""IsError"": <boolean>,
    ""IsErrorUseTable"": <boolean>,
    ""IsGenerate"": <boolean>,
    ""Sql"": ""<string or empty>"",
    ""SqlType"": ""<string or empty>"",
    ""Tables"": [""list of table names referenced or implied""],
    ""Columns"": [""list of column names referenced or implied""]
  }
}

General rules:
- Base all judgments strictly on the input text. Do not assume external context.
- Never reveal internal reasoning or chain-of-thought; only provide a concise high-level summary.

Intent Classification Rules:

1. IsAnalyse = true  
   When the input explicitly requests SQL analysis, explanation, optimization, or classification.

2. IsAnalyseUseTable = true  
   When the analysis references table structure, schema details, columns, indexes, or DDL.

3. IsError = true  
   When the input explicitly asks to find, explain, or fix SQL errors.

4. IsErrorUseTable = true  
   When the error request references table/column/index/schema definitions.

5. IsGenerate = true  
   When the input requests creation or generation of SQL queries or templates.

SQL Extraction Rules:

- Extract the most complete contiguous SQL statement if present. Trim leading/trailing whitespace.
- Concatenate multiple statements with ""; "" if intent clearly targets all.
- If no SQL is present, set Sql to empty string.

SQL Type Inference Rules:

- Infer DB engine only if unambiguous syntax exists (TOP → SQLServer, AUTO_INCREMENT → MySQL, RETURNING → PostgreSQL, PL/SQL blocks → Oracle).
- If uncertain, set SqlType to empty string.

Tables and Columns Extraction Rules:

- Tables: extract all table names explicitly referenced in SQL or implied by the intent (e.g., “Orders table” → Orders).
- Columns: extract all column names explicitly referenced or clearly implied in SQL or intent (e.g., “OrderId”, “amount”).
- If none can be inferred, return empty array [].

Ambiguity fallback:

- If the intent is unclear, set IsAnalyse=false, IsError=false, IsGenerate=false.
- Extract tables/columns only if clearly present or strongly implied; otherwise leave empty arrays.
- Leave SqlType empty if uncertain.

Output Requirements:

- Output exactly the JSON object as defined.
- No extra text, no explanations, no markdown, no extra keys.
- Booleans for flags, strings for Sql and SqlType, arrays for Tables and Columns.

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
