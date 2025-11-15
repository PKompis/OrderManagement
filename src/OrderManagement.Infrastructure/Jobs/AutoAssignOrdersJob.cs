using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Orders.Commands.AutoAssignOrders;
using Quartz;

namespace OrderManagement.Infrastructure.Jobs;

/// <summary>
/// Periodic job that automatically assigns pending delivery orders to available couriers.
/// </summary>
public sealed class AutoAssignOrdersJob(IMediator mediator, ILogger<AutoAssignOrdersJob> logger) : IJob
{
    private const int MaxOrdersPerRun = 5;

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("AutoAssignOrdersJob started at {Time}", DateTimeOffset.UtcNow);

        try
        {
            var command = new AutoAssignOrdersCommand(MaxOrdersPerRun);

            var result = await mediator.Send(command, context.CancellationToken);

            var assignedCount = result.Orders.Count;

            logger.LogInformation("AutoAssignOrdersJob completed. Assigned {Count} orders.", assignedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while running AutoAssignOrdersJob.");
        }
    }
}
