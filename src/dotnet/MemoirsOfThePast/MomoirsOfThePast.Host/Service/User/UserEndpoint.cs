using MemoirsOfThePast.Host.Filters;
using MemoirsOfThePast.HoST.Service.User.Dto;

namespace MemoirsOfThePast.HoST.Service.User
{
    public static class UserEndpoint
    {
        public static void MapUser(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/sysuser")
                    .WithDescription("auth")
                    .WithTags("user")
                    .AddEndpointFilter<ResultEndpointFilter>();

            group.MapPost("register", (IUserService userService, RegisterInput input) => userService.RegisterAsync(input)).WithSummary("注册");
        }
    }
}
