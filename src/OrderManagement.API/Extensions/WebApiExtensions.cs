using Microsoft.OpenApi.Models;
using OrderManagement.API.Filters;
using OrderManagement.Contracts.Common.Errors;
using System.Threading.RateLimiting;

namespace OrderManagement.API.Extensions;

internal static class WebApiExtensions
{
    internal static IServiceCollection AddGlobalRateLimiter(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Global, per-IP
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 1000,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0
                    }));

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                var payload = new ErrorResponse
                {
                    Code = "rate_limited",
                    Message = "Too many requests.",
                    StatusCode = StatusCodes.Status429TooManyRequests
                };
                await context.HttpContext.Response.WriteAsJsonAsync(payload, cancellationToken: token);
            };
        });

    internal static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Order Management API",
                Version = "v1",
                Description = "A .NET 8 minimal API for Order Management System (OMS) for a takeaway restaurant that supports the complete lifecycle of an order, including preparation and delivery."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
            });

            c.OperationFilter<AuthOperationFilter>();
        });
}