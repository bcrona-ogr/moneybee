using Microsoft.AspNetCore.Builder;
using MoneyBee.Shared.API.Middlewares;

namespace MoneyBee.Shared.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}