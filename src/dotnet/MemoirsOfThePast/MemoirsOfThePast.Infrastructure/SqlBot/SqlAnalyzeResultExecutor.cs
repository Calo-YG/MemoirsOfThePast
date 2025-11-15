using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    public class SqlAnalyzeResultExecutor(string id, AIAgent agent) : Executor<SqlMessageAnalyseResult, ChatMessage>(id)
    {
        /// <summary>
        /// 提示词
        /// </summary>
        public const string Prompt = @"";

        /// <summary>
        /// agent 名称
        /// </summary>
        public const string AgentName = "SqlAnalyzeResultExecutor";

        /// <summary>
        /// agent descriptor
        /// </summary>
        public const string Descriptor = "";

        public override ValueTask<ChatMessage> HandleAsync(SqlMessageAnalyseResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
