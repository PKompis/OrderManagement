namespace OrderManagement.Application.Orders.Models;

/// <summary>
/// Order Statistics Model
/// </summary>
public sealed record OrderStatisticsModel
{
    /// <summary>
    /// Gets the total orders.
    /// </summary>
    public int TotalOrders { get; init; }

    /// <summary>
    /// Gets the total pickup orders.
    /// </summary>
    public int TotalPickupOrders { get; init; }

    /// <summary>
    /// Gets the total delivery orders.
    /// </summary>
    public int TotalDeliveryOrders { get; init; }

    /// <summary>
    /// Gets the delivered today.
    /// </summary>
    public int DeliveredToday { get; init; }

    /// <summary>
    /// Gets the total revenue.
    /// </summary>
    public decimal TotalRevenue { get; init; }
}
