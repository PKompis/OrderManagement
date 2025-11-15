using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Customers.Abstractions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Infrastructure.Jobs;
using OrderManagement.Infrastructure.Options;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Customers;
using OrderManagement.Infrastructure.Persistence.Menu;
using OrderManagement.Infrastructure.Persistence.Orders;
using OrderManagement.Infrastructure.Persistence.Staff;
using OrderManagement.Infrastructure.Services;
using Polly;
using Polly.Extensions.Http;
using Quartz;
using static System.Net.WebRequestMethods;

namespace OrderManagement.Infrastructure;

/// <summary>
/// Depedency injection related for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the infrastructure layer depedencies.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "Database:ConnectionString is required")
            .ValidateOnStart()
        .Services
            .AddOptions<OpenRouteServiceOptions>()
            .Bind(configuration.GetSection(OpenRouteServiceOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "OpenRouteService:ApiKey is required")
            .ValidateOnStart()
        .Services
            .AddDbContext<AppDbContext>((sp, opts) =>
            {
                var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                opts.UseSqlServer(dbOptions.ConnectionString!, sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    );

                    sql.CommandTimeout(30);
                });
            })
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<ICustomerReadRepository, CustomerReadRepository>()
            .AddScoped<IMenuItemReadRepository, MenuItemReadRepository>()
            .AddScoped<IMenuItemRepository, MenuItemRepository>()
            .AddScoped<IOrderReadRepository, OrderReadRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IStaffReadRepository, StaffReadRepository>()
            .AddQuartz(q =>
            {
                var jobKey = new JobKey("AutoAssignOrdersJob");

                q.AddJob<AutoAssignOrdersJob>(opts => opts
                    .WithIdentity(jobKey)
                    .WithDescription("Automatically assigns pending delivery orders to available couriers."));

                q.AddTrigger(t => t
                    .ForJob(jobKey)
                    .WithIdentity("AutoAssignOrdersJob-trigger")
                    .WithSimpleSchedule(s => s
                        .WithIntervalInMinutes(5)
                        .RepeatForever()));
            })
            .AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            })
            .AddHttpClient<IDeliveryEtaService, OpenRouteServiceEtaService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<OpenRouteServiceOptions>>().Value;

                if (!string.IsNullOrWhiteSpace(options.BaseUrl))
                {
                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(10);
                }
            })
            .AddPolicyHandler(policy =>
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        3,
                        retry => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retry))
                    )
            )
        .Services;
}