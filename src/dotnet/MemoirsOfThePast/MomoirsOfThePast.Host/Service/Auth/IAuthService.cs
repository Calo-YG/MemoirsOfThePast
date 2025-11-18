using MemoirsOfThePast.HoST.Service.Auth.Dto;

namespace MemoirsOfThePast.HoST.Service.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<LoginDto> LoginAsync(LoginInput input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> RefreshTokenAsync(RefreshInput input);
    }
}
