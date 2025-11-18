using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.JwtAuthentication;

namespace MemoirsOfThePast.HoST.Service.Memory
{
    /// <summary>
    /// 回忆 Memory
    /// </summary>
    public class MemoryService(IDbContext dbContext,IUserContext userContext):IMemoryService
    {
    }
}
