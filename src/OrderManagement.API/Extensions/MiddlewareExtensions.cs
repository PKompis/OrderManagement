using OrderManagement.API.Middlewares;

namespace OrderManagement.API.Extensions;

internal static class MiddlewareExtensions
{
    internal static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
