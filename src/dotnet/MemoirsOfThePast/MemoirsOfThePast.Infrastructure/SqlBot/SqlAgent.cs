using MemoirsOfThePast.Infrastructure.Options;
using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <param name="options"></param>
    /// <param name="sqlMessageAnalyzeAgent">
    /// SqlMessageAnalyzeAgent
    /// </param>
    public class SqlAgent(IOptionsSnapshot<DbOptions> options,ILoggerFactory loggerFactory,
        [FromKeyedServices(SqlMessageAnalyzeExecutor.AgentName)] AIAgent sqlMessageAnalyzeAgent) : ISqlAgent
    {
        /// <summary>
        /// 数据库连接设置
        /// </summary>
        private readonly DbOptions dbOptions = options.Value;

        /// <summary>
        /// 
        /// </summary>
        private readonly SqlMessageAnalyzeExecutor sqlMessageAnalyze = new SqlMessageAnalyzeExecutor("SqlMessageAnalyzeExecutor", aIAgent: sqlMessageAnalyzeAgent, loggerFactory.CreateLogger<SqlMessageAnalyzeExecutor>());

        /// <summary>
        /// 创建工作流
        /// </summary>
        /// <returns></returns>
        public Workflow CreateWorkflow()
        {
            return new WorkflowBuilder(sqlMessageAnalyze).Build();
        }

        /// <summary>
        /// 将工作流转换成AIAgent
        /// </summary>
        /// <returns></returns>
        public AIAgent CreateAIAgent()
        {
            return new WorkflowBuilder(sqlMessageAnalyzeAgent).Build().AsAgent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AIAgent CreateMessageAnalyzeAIAgent(IChatClient chatClient,ILoggerFactory loggerFactory)
        {
            var chatClientOptions = new ChatClientAgentOptions()
            {
                Instructions = SqlMessageAnalyzeExecutor.Prompt,
                Description = SqlMessageAnalyzeExecutor.Descriptor,
                Name = SqlMessageAnalyzeExecutor.AgentName,
                ChatOptions = new()
                {
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<SqlMessageAnalyseResult>()
                }
            };
            return chatClient.CreateAIAgent(options: chatClientOptions, loggerFactory: loggerFactory);
        }
    }
}
