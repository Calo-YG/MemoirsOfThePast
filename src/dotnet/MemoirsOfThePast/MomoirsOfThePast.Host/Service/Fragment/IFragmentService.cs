using MemoirsOfThePast.HoST.Service.Fragment.Dto;

namespace MemoirsOfThePast.HoST.Service.Fragment
{
    public interface IFragmentService
    {
        Task AddFragmentAsync(CreateFramentInput input);

        Task<List<FramentListDto>> GetListAsync(FragmentListInput input);
    }
}
