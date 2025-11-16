using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Infrastructure.Persistence.Orders;

/// <summary>
/// Order Repository
/// </summary>
/// <seealso cref="IOrderRepository" />
public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    /// <summary>
    /// Gets the order by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return default;

        return await dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets delivery orders that are pending assignment:
    /// - Delivery type
    /// - No courier assigned
    /// - Status = Pending or ReadyForDelivery
    /// </summary>
    /// <param name="maxResults"></param>
    /// <param name="cancellationToken"></param>
    public async Task<List<Order>> GetPendingAssignmentOrdersAsync(int? maxResults = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = dbContext.Orders
            .Where(o =>
                o.Type == OrderType.Delivery &&
                o.Assignment == null &&
                (o.Status == OrderStatus.Pending || o.Status == OrderStatus.ReadyForDelivery))
            .OrderBy(o => o.CreatedAt);

        if (maxResults.HasValue) query = query.Take(maxResults.Value);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds to the order asynchronous.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default) => await dbContext.Orders.AddAsync(order, cancellationToken);

    /// <summary>
    /// Updates the order asynchronous.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Update(order);

        return Task.CompletedTask;
    }
}