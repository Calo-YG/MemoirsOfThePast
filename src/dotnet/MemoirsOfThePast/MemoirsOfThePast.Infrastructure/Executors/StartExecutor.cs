using Microsoft.Agents.AI.Workflows;
using System.Text;

namespace MemoirsOfThePast.Infrastructure.Executor
{
    public record class StartExecutor : ExecutorBinding
    {
        public StartExecutor(string Id, Func<string, ValueTask<Microsoft.Agents.AI.Workflows.Executor>> FactoryAsync, Type ExecutorType, object RawValue = null) : base(Id, FactoryAsync, ExecutorType, RawValue)
        {
        }

        public override bool IsSharedInstance => throw new NotImplementedException();

        public override bool SupportsConcurrentSharedExecution => throw new NotImplementedException();

        public override bool SupportsResetting => throw new NotImplementedException();

        protected override bool PrintMembers(StringBuilder builder)
        {
            return base.PrintMembers(builder);
        }

        protected override ValueTask<bool> ResetCoreAsync()
        {
            return base.ResetCoreAsync();
        }
    }
}
