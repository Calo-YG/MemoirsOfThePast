using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <summary>
    /// chat client 客户端
    /// </summary>
    /// <param name="chatClient"></param>
    public class SqlExecuteExecutor(IChatClient chatClient) : Executor<ChatMessage, string>("SqlExecuteExecutor")
    {
        private const string Prompt = $"";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ValueTask<string> HandleAsync(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
