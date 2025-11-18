using MemoirsOfThePast.HoST.Service.Memory.Dto;
using MemoirsOfThePast.Infrastructure.Domain;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.JwtAuthentication;
using Microsoft.EntityFrameworkCore;

namespace MemoirsOfThePast.HoST.Service.Memory
{
    /// <summary>
    /// 回忆 Memory
    /// </summary>
    public class MemoryService(IDbContext dbContext,IUserContext userContext):IMemoryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddMemoryAsync(CreateMemoryInput input)
        {
            var userId = userContext.UserId;

            var entity = new MemoryEntity
            {
                 Id = userId,
                 Description = input.Description,
                 Avatar = input.Avatar,
                 Background = input.Background,
                 Name = input.Name,
                 UserId = userId,
                 CreateDate = DateTime.Now,
            };

            await dbContext.Memories.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<List<MemoryListDto>> GetListAsync(string name)
        {
            var userId = userContext.UserId;

            return dbContext.Memories.AsNoTracking()
                .Where(p=>p.UserId == userId)
                .WhereIf(!string.IsNullOrEmpty(name),p=>p.Name.Contains(name)).Select(p=> new MemoryListDto
                {
                    Id=p.Id,
                    Description = p.Description,
                    Avatar=p.Avatar,
                    Background=p.Background,
                    Name = p.Name
                }).ToListAsync();
        }
    }
}
