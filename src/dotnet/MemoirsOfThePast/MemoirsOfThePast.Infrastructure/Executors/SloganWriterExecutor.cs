using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace MemoirsOfThePast.Infrastructure.Executors
{
    public sealed class SloganWriterExecutor : Executor<string>
    {
        private readonly AIAgent agent;

        private readonly AgentThread thread;

        public SloganWriterExecutor(string id,IChatClient client, ExecutorOptions options = null, bool declareCrossRunShareable = false) : base(id, options, declareCrossRunShareable)
        {

        }

        public override ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
