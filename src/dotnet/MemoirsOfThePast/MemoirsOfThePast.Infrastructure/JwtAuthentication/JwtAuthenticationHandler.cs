using MemoirsOfThePast.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtOptions>
    {
        private readonly ITokenManageService tokenManageService;

        public JwtAuthenticationHandler(ITokenManageService tokenManageService, IOptionsMonitor<JwtOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
            this.tokenManageService = tokenManageService;
        }

        /// <summary>
        /// 实现鉴权机制
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await tokenManageService.IsAuthenticatedAsync(Context);

            if (!result.IsAuthenticated)
            {
                return AuthenticateResult.Fail(new BusinessException(result.Code, result.Message));
            }

            var claims = tokenManageService.GetClaims(Context);

            ClaimsIdentity[] identity = [new ClaimsIdentity(claims,authenticationType:JwtBearerConst.Scheme)];

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal,JwtBearerConst.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
