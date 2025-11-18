using MemoirsOfThePast.Host.Filters;
using MemoirsOfThePast.HoST.Service.Fragment.Dto;
using Microsoft.AspNetCore.Builder;

namespace MemoirsOfThePast.HoST.Service.Fragment
{
    public static class FragmentEndpoint
    {
        public static void MapFragment(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/fragment")
                .WithDescription("fragment")
                .WithTags("fragment")
                .AddEndpointFilter<ResultEndpointFilter>();

            group.MapPost("addFragment",(IFragmentService fragementService,CreateFramentInput input)=> fragementService.AddFragmentAsync(input)).WithSummary("创建碎片").RequireAuthorization();

            group.MapPost("getList",(IFragmentService fragementService, FragmentListInput input)=> fragementService.GetListAsync(input)).WithSummary("碎片列表").RequireAuthorization();
        }
    }
}
