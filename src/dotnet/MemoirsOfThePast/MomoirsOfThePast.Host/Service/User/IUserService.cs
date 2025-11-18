using MemoirsOfThePast.HoST.Service.User.Dto;

namespace MemoirsOfThePast.HoST.Service.User
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterInput input);
    }
}
