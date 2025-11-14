using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotEvent;
using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
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
        /// chatCliennt 客户端
        /// </summary>
        private readonly IChatClient _chatClient;

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

        private const string Prompt = @"You are a SQL semantic analysis assistant. Your task is to analyze a user-provided text and classify their intent related to SQL. You must return a structured JSON output only, without any explanations or additional commentary.  

The classifications are:

1. **IsAnalyse**: true if the user is performing SQL performance analysis; false otherwise.
2. **IsAnalyseUseTable**: true if the user is performing SQL performance analysis and is using table structure information; false otherwise.
3. **IsError**: true if the user is performing SQL error analysis; false otherwise.
4. **IsErrorUseTable**: true if the user is performing SQL error analysis and is using table structure information; false otherwise.
5. **IsGenerate**: true if the user intends to generate SQL statements; false otherwise.

Rules for determining values:
- Only rely on the text provided.
- If the user explicitly or implicitly mentions table structures (schemas, columns, indexes) in the context of performance analysis, set `IsAnalyseUseTable` to true.
- If the user explicitly or implicitly mentions table structures in the context of error analysis, set `IsErrorUseTable` to true.
- Treat each field independently, but all fields must be included in the output JSON.
- Do not add extra explanation or commentary. Only return the JSON object.

Output format:
```json
{
  ""IsAnalyse"": <true/false>,
  ""IsAnalyseUseTable"": <true/false>,
  ""IsError"": <true/false>,
  ""IsErrorUseTable"": <true/false>,
  ""IsGenerate"": <true/false>
}
";

        public SqlMessageAnalyzeExecutor(string id,IChatClient chatClient,ILoggerFactory loggerFactory):base(id) 
        { 
            _chatClient = chatClient;
            _logger = loggerFactory.CreateLogger<SqlMessageAnalyzeExecutor>();
            var chatClientOptions = new ChatClientAgentOptions()
            {
               Instructions = Prompt,
               Description = "You are a SQL semantic analysis assistant. Your task is to analyze a user-provided text and classify their intent related to SQL. You must return a structured JSON output only, without any explanations or additional commentary.",
               Name = "sqlanalyse",
                ChatOptions = new()
                {
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<SqlMessageAnalyseResult>()
                }
            };
            aIAgent = chatClient.CreateAIAgent(options:chatClientOptions,loggerFactory:loggerFactory);
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

            _logger.LogInformation("Agent Output");

            await context.AddEventAsync(new SqlMessageAnalyseEvent(sloganResult), cancellationToken);

            return sloganResult;
        }
    }
}
