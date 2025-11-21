using Microsoft.Agents.AI;

namespace MemoirsOfThePast.Infrastructure.Executors.Abstract
{
    /// <summary>
    /// 消息传递上下文
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ExecutorMessageContext<T> where T : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public T Message { get; private set; }

        public ExecutorMessageContext(T message) 
        {
            this.Message = message; 
        }
    }
}
