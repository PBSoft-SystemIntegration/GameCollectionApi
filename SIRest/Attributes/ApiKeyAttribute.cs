using GameCollectionApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GameCollectionApi.Attributes
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var service = context.HttpContext.RequestServices.GetRequiredService<IApiKeyService>();
            if (!context.HttpContext.Request.Headers.TryGetValue("x-api-key", out var key))
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "Missing Api Key in Header!"
                };
                return;
            }
            if (!await service.ValidateAsync(key.ToString()))
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "Invalid Api key. You do not have access to this enpoint"
                };
                return;
            }
            await next();
        }
    }

}
