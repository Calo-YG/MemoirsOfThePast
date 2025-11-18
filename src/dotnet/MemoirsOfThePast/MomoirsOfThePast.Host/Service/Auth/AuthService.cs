using MemoirsOfThePast.HoST.Service.Auth.Dto;
using MemoirsOfThePast.Infrastructure.Const;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.Exceptions;
using MemoirsOfThePast.Infrastructure.JwtAuthentication;
using MemoirsOfThePast.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MemoirsOfThePast.HoST.Service.Auth
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="distributedCache"></param>
    public class AuthService(IDbContext dbContext,IDistributedCache distributedCache,ITokenManageService tokenManageService):IAuthService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<LoginDto> LoginAsync(LoginInput input)
        {
            var entity = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(p=>p.Account == input.Account);

            if (entity is null)
            {
                throw new BusinessException("账号密码输入错误");
            }

            var password = AesUtil.Encrypt(input.Password, entity.Solt, EncryptConst.AESIV);

            if(password != entity.Password)
            {
                throw new BusinessException("账号密码输入错误");
            }

            var cliams = new List<Claim>()
            {
                new Claim(JwtConst.UserId, entity.Id),
                new Claim(JwtConst.UserName,entity.Name),
            };

            var token = tokenManageService.GenerateToken(cliams);

            var refreshToken = tokenManageService.GenerateRefreshToken(cliams);

            var dto = new LoginDto
            {
                Token = token.Token,
                RefreshToken = refreshToken,
            };

            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20)
            };

            var cacheKey = string.Format(CacheKeyConst.TokenCacheKey, entity.Id);
            var refreshTokenKey = string.Format(CacheKeyConst.RefreshTokenCacheKey, entity.Id);

            await distributedCache.SetStringAsync(cacheKey, token.Token, cacheEntryOptions);
            await distributedCache.SetStringAsync(refreshTokenKey, refreshToken);

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> RefreshTokenAsync(RefreshInput input)
        {
            var cacheKey = string.Format(CacheKeyConst.RefreshTokenCacheKey, input.UserId);

            var cacheTokenStr = distributedCache.GetString(cacheKey);

            if (string.IsNullOrEmpty(cacheTokenStr))
            {
                throw new SecurityTokenExpiredException("登录过期，请重新登录");
            }

            if (input.RefreshToken != cacheTokenStr)
            {
                throw new SecurityTokenExpiredException("登录已失效,其它设备登录");
            }

            var validate = tokenManageService.ValidateJwtToken(input.RefreshToken, out var securityToken);

            if (validate != null)
            {
                throw new SecurityTokenExpiredException("刷新token错误");
            }

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var cliams = jwtSecurityTokenHandler.ReadJwtToken(input.RefreshToken).Claims;

            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20)
            };

            var accessToken = tokenManageService.GenerateToken(cliams);

            var tokenCacheKey = string.Format(CacheKeyConst.TokenCacheKey, input.UserId);

            await distributedCache.SetStringAsync(tokenCacheKey, accessToken.Token, cacheEntryOptions);

            return accessToken.Token;
        }
    }
}
