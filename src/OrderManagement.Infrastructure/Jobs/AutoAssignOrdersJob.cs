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

    /// <summary>
    /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
    /// fires that is associated with the <see cref="T:Quartz.IJob" />.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <remarks>
    /// The implementation may wish to set a  result object on the
    /// JobExecutionContext before this method exits.  The result itself
    /// is meaningless to Quartz, but may be informative to
    /// <see cref="T:Quartz.IJobListener" />s or
    /// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
    /// execution.
    /// </remarks>
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
