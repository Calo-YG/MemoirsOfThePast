using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    internal class SqlExecuteExecutor() : Executor<ChatMessage, string>("SqlExecuteExecutor")
    {
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
