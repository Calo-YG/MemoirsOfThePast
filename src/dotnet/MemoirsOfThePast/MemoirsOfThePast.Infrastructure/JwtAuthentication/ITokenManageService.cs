using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// Token 管理
    /// </summary>
    public  interface ITokenManageService
    {
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        (string Token, DateTime ExpiredTime) GenerateToken(IEnumerable<Claim> claims);

        /// <summary>
        /// 生成刷新token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateRefreshToken(IEnumerable<Claim> claims);

        /// <summary>
        /// 判断鉴权是否通过
        /// </summary>
        Task<JwtAuthenticateResult> IsAuthenticatedAsync(HttpContext context);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        IEnumerable<Claim> GetClaims(HttpContext context);

        /// <summary>
        /// 校验token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="jwtSecurityToken"></param>
        /// <returns></returns>
        Exception ValidateJwtToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}
