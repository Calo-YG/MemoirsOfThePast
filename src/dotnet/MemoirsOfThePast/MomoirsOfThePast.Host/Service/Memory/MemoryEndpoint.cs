using MemoirsOfThePast.Host.Filters;
using MemoirsOfThePast.HoST.Service.Memory.Dto;


namespace MemoirsOfThePast.HoST.Service.Memory
{
    public static class MemoryEndpoint
    {
        public static void MapMemory(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/memory")
                 .WithDescription("memory")
                 .WithTags("memory")
                 .AddEndpointFilter<ResultEndpointFilter>();

            group.MapPost("addMemroy", (IMemoryService memoryService, CreateMemoryInput input) => memoryService.AddMemoryAsync(input)).WithSummary("创建回忆").RequireAuthorization();

            group.MapGet("getList",(IMemoryService memoryService,string name)=> memoryService.GetListAsync(name)).WithSummary("回忆列表").RequireAuthorization();
        }
    }
}
