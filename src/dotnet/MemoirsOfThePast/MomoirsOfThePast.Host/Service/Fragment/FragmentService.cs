using MemoirsOfThePast.HoST.Service.Fragment.Dto;
using MemoirsOfThePast.Infrastructure.Domain;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.JwtAuthentication;
using Microsoft.EntityFrameworkCore;


namespace MemoirsOfThePast.HoST.Service.Fragment
{
    public class FragmentService(IDbContext dbContext,IUserContext userContext): IFragmentService
    {
        public async Task AddFragmentAsync(CreateFramentInput input)
        {
            var userId = userContext.UserId;

            var entity = new FragmentEntity
            {
                Id = Guid.NewGuid().ToString("N"),
                MemoryId = input.MemoryId,
                Description = input.Description,
                OccurDate = input.OccurDate,
                Location = input.Location,
                Scene = input.Scene,
                CreateDate = DateTime.Now
            };

            await dbContext.Fragments.AddAsync(entity); 
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<List<FramentListDto>> GetListAsync(FragmentListInput input)
        {
            return dbContext.QueryNoTracking<FragmentEntity>()
                .Where(p => p.MemoryId == input.MemoryId)
                .WhereIf(!string.IsNullOrEmpty(input.Name), p => p.Description.Contains(input.Name) || p.Scene.Contains(input.Name))
                .Select(p => new FramentListDto
                {
                    Description = p.Description,
                    OccurDate= p.OccurDate,
                    Id = p.Id,
                    Scene = p.Scene,
                    Location = p.Location,
                }).ToListAsync();
        }
    }
}
