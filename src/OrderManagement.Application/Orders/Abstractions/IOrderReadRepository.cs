using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.Entities;

namespace OrderManagement.Application.Orders.Abstractions;

/// <summary>
/// Main interface for read operations on orders
/// </summary>
public interface IOrderReadRepository
{
    /// <summary>
    /// Gets the order by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the order by filter asynchronous.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyList<Order>> GetByFilterAsync(OrderFilter filter, int? maxResults = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the statistics asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<OrderStatisticsModel> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
