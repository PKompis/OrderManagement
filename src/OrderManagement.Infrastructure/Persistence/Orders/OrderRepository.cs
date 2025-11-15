using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Domain.Orders.Entities;

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