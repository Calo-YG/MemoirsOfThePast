using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace MemoirsOfThePast.Infrastructure.Agents
{
    public class MemoryAgent(IChatClient chatClient,IDbContext context)
    {
    }
}
