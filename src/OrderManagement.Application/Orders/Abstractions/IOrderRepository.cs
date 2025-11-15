using OrderManagement.Domain.Orders.Entities;

namespace OrderManagement.Application.Orders.Abstractions;

/// <summary>
/// Main interface for order general operations
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Gets the order by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds to the order asynchronous.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the order asynchronous.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}
