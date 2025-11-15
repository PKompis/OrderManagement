namespace OrderManagement.Application.Admin.Queries;

/// <summary>
/// Order Statistics Result
/// </summary>
public sealed record OrderStatisticsResult
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
