using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// jwt 扩展类
    /// </summary>
    public static class JwtBearerExtensions
    {
        /// <summary>
        /// 添加jwt
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration,string authenticationScheme = JwtBearerConst.Scheme)
        {
            builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

            var options = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

            builder.Services.AddScoped<ITokenManageService, TokenManageService>();

            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizeResultHandle>();

            return builder.AddScheme<JwtOptions, JwtAuthenticationHandler>(authenticationScheme, authenticationScheme, p =>
            {
                p.Audience = options.Audience;
                p.ClaimsIssuer = options.ClaimsIssuer;
                p.ExpirationTime = options.ExpirationTime;
                p.Issuer = options.Issuer;
                p.RefreshExpirationTime = options.RefreshExpirationTime;
                p.SymmetricSecurityKey = options.SymmetricSecurityKey;
            });
        }
    }
}
