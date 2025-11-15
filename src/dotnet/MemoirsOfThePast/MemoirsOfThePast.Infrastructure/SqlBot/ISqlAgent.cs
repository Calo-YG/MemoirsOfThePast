using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    public interface ISqlAgent
    {
        /// <summary>
        /// 创建工作流
        /// </summary>
        /// <returns></returns>
        Workflow CreateWorkflow();

        /// <summary>
        /// 基于工作流创建AIAgent
        /// </summary>
        /// <returns></returns>
        AIAgent CreateAIAgent();
    }
}
