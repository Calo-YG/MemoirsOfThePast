using MemoirsOfThePast.Infrastructure.Core;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MemoirsOfThePast.Host.Filters
{
    public class ResultEndpointFilter : IEndpointFilter
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var result = await next(context);

            if (result is not FileStreamHttpResult)
            {
                return ApiResult.Success(result);
            }

            return result;
        }
    }
}
