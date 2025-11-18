using MemoirsOfThePast.Host.Filters;
using MemoirsOfThePast.HoST.Service.Auth.Dto;

namespace MemoirsOfThePast.HoST.Service.Auth
{
    public static class AuthEndpoint
    {
        public static void MapAuth(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/auth")
                  .WithDescription("auth")
                  .WithTags("auth")
                  .AddEndpointFilter<ResultEndpointFilter>();

            group.MapPost("login",(IAuthService authService,LoginInput input) =>authService.LoginAsync(input)).WithSummary("登录");
            group.MapPost("refreshTokn", (IAuthService authService, RefreshInput input) => authService.RefreshTokenAsync(input)).WithSummary("刷新token");
        }
    }
}
