using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Menu.Mappings;
using OrderManagement.Application.Orders.Mappings;
using OrderManagement.Application.Pipelines;

namespace OrderManagement.Application;

/// <summary>
/// Depedency injection related for Application layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the application layer depedencies.
    /// </summary>
    /// <param name="services">The services.</param>
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly))
            .AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MenuApplicationProfile>();
                cfg.AddProfile<OrdersApplicationProfile>();
            });
}
