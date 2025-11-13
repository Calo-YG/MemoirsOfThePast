using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MemoirsOfThePast.Infrastructure.AgentMiddleware
{
    /// <summary>
    /// token 计算 中间件
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="thread"></param>
    /// <param name="options"></param>
    /// <param name="innerAgent"></param>
    /// <param name="cancellationToken"></param>
    internal class TokenCaluteMiddleware(IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
    {

    }
}
