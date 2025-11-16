using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Infrastructure.Persistence.Orders;

/// <summary>
/// Order Read Repository
/// </summary>
/// <seealso cref="IOrderReadRepository" />
public sealed class OrderReadRepository(AppDbContext dbContext) : IOrderReadRepository
{
    /// <summary>
    /// Gets the order by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return default;

        return await dbContext.Orders.AsNoTracking().Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets the order by filter asynchronous.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<IReadOnlyList<Order>> GetByFilterAsync(OrderFilter filter, int? maxResults = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = dbContext.Orders.AsNoTracking().Include(o => o.Items);

        if (filter.Type is not null) query = query.Where(o => o.Type == filter.Type);

        if (filter.Status is not null) query = query.Where(o => o.Status == filter.Status);

        if (filter.AssignedCourierId is not null) query = query.Where(o => o.Assignment != null && o.Assignment.CourierId == filter.AssignedCourierId);

        if (filter.CustomerId is not null) query = query.Where(o => o.CustomerId == filter.CustomerId);

        query = query.OrderBy(o => o.CreatedAt);

        if (maxResults is not null) query = query.Take(maxResults.Value);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the statistics asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<OrderStatisticsModel> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var orders = dbContext.Orders.AsNoTracking();

        var totalOrders = await orders.CountAsync(cancellationToken);
        var totalPickupOrders = await orders.Where(o => o.Type == OrderType.Pickup).CountAsync(cancellationToken);
        var totalDeliveryOrders = await orders.Where(o => o.Type == OrderType.Delivery).CountAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var todayStart = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
        var tomorrowStart = todayStart.AddDays(1);

        var deliveredToday = await orders
            .Where(o =>
                o.Status == OrderStatus.Delivered &&
                o.CreatedAt >= todayStart &&
                o.CreatedAt < tomorrowStart)
            .CountAsync(cancellationToken);

        var totalRevenue = await orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SelectMany(o => o.Items)
            .SumAsync(i => i.UnitPrice * i.Quantity, cancellationToken);

        return new OrderStatisticsModel
        {
            TotalOrders = totalOrders,
            TotalPickupOrders = totalPickupOrders,
            TotalDeliveryOrders = totalDeliveryOrders,
            DeliveredToday = deliveredToday,
            TotalRevenue = totalRevenue
        };
    }
}