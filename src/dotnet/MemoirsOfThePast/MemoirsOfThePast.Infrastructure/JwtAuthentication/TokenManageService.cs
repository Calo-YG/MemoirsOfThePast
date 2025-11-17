using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// token管理
    /// </summary>
    /// <param name="distributedCache"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public class TokenManageService(IDistributedCache distributedCache, IOptionsSnapshot<JwtOptions> options, ILogger<ITokenManageService> logger) : ITokenManageService
    {
        /// <summary>
        /// JWT 令牌处理
        /// </summary>
        private readonly JwtOptions JwtOptions = options.Value;

        /// <summary>
        /// JWT 安全令牌处理
        /// </summary>
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        /// <summary>
        /// Clams
        /// </summary>
        private IEnumerable<Claim> Claims = Enumerable.Empty<Claim>();


        /// <summary>
        /// 是否通过鉴权
        /// </summary>
        /// <exception cref="UserNotLoginException"></exception>
        public async Task<JwtAuthenticateResult> IsAuthenticatedAsync(HttpContext context)
        {
            var token = string.Empty;

            var getHeadAuthorization = context.Request.Headers.TryGetValue("Authorization", out var headAuthorization);

            var getQueryAuthorization = context.Request.Query.TryGetValue("accessToken", out var queryAuthorization);

            if (getHeadAuthorization && !string.IsNullOrEmpty(headAuthorization))
            {
                token = headAuthorization;
            }

            if (getHeadAuthorization && !string.IsNullOrEmpty(queryAuthorization))
            {
                token = queryAuthorization;
            }

            var result = new JwtAuthenticateResult
            {
                IsAuthenticated = false
            };

            token = token.Replace("Bearer ", "");

            var claims = GetClaims(context);

            var userId = string.Empty;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                result.Code = 401;
                result.Message = "token校验失败,请求头未携带token";
                return result;
            }

            var validateToken = ValidateJwtToken(token, out var jwtSecurityToken);

            if (validateToken != null)
            {
                logger.LogError($"校验token 失败：{validateToken?.Message}");

                if (validateToken is SecurityTokenExpiredException)
                {
                    result.Code = 401;
                    result.Message = "token过期,请重新登录";
                }
                else
                {
                    result.Code = 401;
                    result.Message = "token校验失败,请重新登录";
                }

                return result;
            }

            var cacheTokenStr = await distributedCache.GetStringAsync(string.Format("",userId));

            if (string.IsNullOrEmpty(cacheTokenStr))
            {
                result.Code = 401;
                result.Message = "登录过期，请重新登录";
                return result;
            }

            var cacheToken = JsonSerializer.Deserialize<TokenModel>(cacheTokenStr);

            if (cacheToken.AccessToken != token)
            {
                result.Code = 401;
                result.Message = "设备在别处登录，请重新登录";
                return result;
            }

            result.IsAuthenticated = true;

            return result;
        }

        /// <summary>
        /// 获取token Claims
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Claim> GetClaims(HttpContext context)
        {
            if (Claims.Any())
            {
                return Claims;
            }

            var token = string.Empty;

            var getHeadAuthorization = context.Request.Headers.TryGetValue("Authorization", out var headAuthorization);

            var getQueryAuthorization = context.Request.Query.TryGetValue("accessToken", out var queryAuthorization);

            if (getHeadAuthorization && !string.IsNullOrEmpty(headAuthorization))
            {
                token = headAuthorization;
            }

            if (getHeadAuthorization && !string.IsNullOrEmpty(queryAuthorization))
            {
                token = queryAuthorization;
            }

            if (string.IsNullOrEmpty(token) || token == "Anonymous")
            {
                return Claims;
            }

            token = token.Replace("Bearer ", "");

            Claims = jwtSecurityTokenHandler.ReadJwtToken(token).Claims;

            return Claims;
        }

        /// <summary>
        /// 验证JwtToken
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="jwtSecurityToken">JwtToken</param>
        public Exception ValidateJwtToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            try
            {
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SymmetricSecurityKey));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//验证颁发者
                    ValidateAudience = true,//验证接收者
                    ValidateLifetime = true,//验证过期时间
                    ValidateIssuerSigningKey = true, //是否验证签名
                    ValidIssuer = JwtOptions.Issuer,//颁发者
                    ValidAudience = JwtOptions.Audience,//接收者
                    IssuerSigningKey = symmetricSecurityKey,//解密密钥
                    ClockSkew = TimeSpan.Zero //缓冲时间
                };

                jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                jwtSecurityToken = validatedToken as JwtSecurityToken;

                return null;
            }
            catch (Exception ex)
            {
                jwtSecurityToken = null;

                return ex;
            }
        }


        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="expirationTime">过期时间</param>
        /// <param name="claims">自定义Claims</param>
        /// <returns></returns>
        private string GenerateJwtToken(DateTime? expirationTime, IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SymmetricSecurityKey));
            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expirationTime,
                signingCredentials: creds);
            return jwtSecurityTokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 生成刷新token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GenerateRefreshToken(IEnumerable<Claim> claims)
        {
            var expiredTime = DateTime.Now.AddMinutes(JwtOptions.RefreshExpirationTime);

            return GenerateJwtToken(expiredTime, claims);
        }

        /// <summary>
        /// 生成访问token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public (string Token, DateTime ExpiredTime) GenerateToken(IEnumerable<Claim> claims)
        {
            var expiredTime = DateTime.Now.AddMinutes(JwtOptions.ExpirationTime);
            var token = GenerateJwtToken(expiredTime, claims);
            return (token, expiredTime);
        }
    }
}