using MemoirsOfThePast.HoST.Service.Memory.Dto;

namespace MemoirsOfThePast.HoST.Service.Memory
{
    public interface IMemoryService
    {
        Task AddMemoryAsync(CreateMemoryInput input);

        Task<List<MemoryListDto>> GetListAsync(string name);
    }
}
