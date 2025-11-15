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
        [FromKeyedServices(SqlMessageAnalyzeExecutor.AgentName)] AIAgent sqlMessageAnalyzeAgent,
        [FromKeyedServices(SqlErrorAmendExecutor.AgentName)] AIAgent sqlErrorAmendAgent,
        [FromKeyedServices(SqlPerformanceExecutor.AgentName)] AIAgent sqlPerfomanceAgent) : ISqlAgent
    {
        /// <summary>
        /// 数据库连接设置
        /// </summary>
        private readonly DbOptions dbOptions = options.Value;

        /// <summary>
        /// sql 语义分析助手
        /// </summary>
        private readonly SqlMessageAnalyzeExecutor sqlMessageAnalyze = new SqlMessageAnalyzeExecutor("SqlMessageAnalyzeExecutor", aIAgent: sqlMessageAnalyzeAgent, loggerFactory.CreateLogger<SqlMessageAnalyzeExecutor>());

        /// <summary>
        /// 错误 sql 修正
        /// </summary>
        private readonly SqlErrorAmendExecutor sqlErrorAmend = new SqlErrorAmendExecutor("SqlErrorAmendExecutor",agent:sqlErrorAmendAgent,loggerFactory.CreateLogger<SqlErrorAmendExecutor>());

        /// <summary>
        /// sql 性能分析优化
        /// </summary>
        private readonly SqlPerformanceExecutor sqlPerformance = new SqlPerformanceExecutor("SqlPerformanceExecutor", agent: sqlPerfomanceAgent, loggerFactory.CreateLogger<SqlPerformanceExecutor>());

        /// <summary>
        /// 创建工作流
        /// </summary>
        /// <returns></returns>
        public Workflow CreateWorkflow()
        {
            return new WorkflowBuilder(sqlMessageAnalyze)
                .AddEdge<SqlMessageAnalyseResult>(sqlMessageAnalyze,sqlErrorAmend,condition:p=> p.Result.IsError)
                .AddEdge<SqlMessageAnalyseResult>(sqlMessageAnalyze,sqlPerformance,condition:p=>p.Result.IsAnalyse)
                .Build();
        }

        /// <summary>
        /// 将工作流转换成AIAgent
        /// </summary>
        /// <returns></returns>
        public AIAgent CreateAIAgent()
        {
            return new WorkflowBuilder(sqlMessageAnalyzeAgent)
                .AddEdge<SqlMessageAnalyseResult>(sqlMessageAnalyze, sqlErrorAmend, condition: p => p.Result.IsError)
                .Build()
                .AsAgent();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatClient"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static AIAgent CreateAmendAIAgent(IChatClient chatClient, ILoggerFactory loggerFactory)
        {
            var chatClientOptions = new ChatClientAgentOptions()
            {
                Instructions = SqlErrorAmendExecutor.Prompt,
                Description = SqlErrorAmendExecutor.Descriptor,
                Name = SqlErrorAmendExecutor.AgentName
            };
            return chatClient.CreateAIAgent(options: chatClientOptions, loggerFactory: loggerFactory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatClient"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static AIAgent CreatePerformanceAIAgent(IChatClient chatClient,ILoggerFactory loggerFactory)
        {
            var chatClientOptions = new ChatClientAgentOptions()
            {
                Instructions = SqlPerformanceExecutor.Prompt,
                Description = SqlPerformanceExecutor.Descriptor,
                Name = SqlPerformanceExecutor.AgentName
            };
            return chatClient.CreateAIAgent(options: chatClientOptions, loggerFactory: loggerFactory);
        }
    }
}
