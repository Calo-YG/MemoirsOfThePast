using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotExecutor;
using Microsoft.Agents.AI.Workflows;

namespace MemoirsOfThePast.Infrastructure.SqlBot.SqlBotEvent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    public class SqlMessageAnalyseEvent(SqlMessageAnalyseResult result) : WorkflowEvent(result)
    {

    }
}
