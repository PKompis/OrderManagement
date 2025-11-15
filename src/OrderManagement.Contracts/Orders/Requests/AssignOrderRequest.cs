namespace OrderManagement.Contracts.Orders.Requests;

/// <summary>
/// Assign Order Request DTO
/// </summary>
public sealed record AssignOrderRequest
{
    /// <summary>
    /// Gets the courier identifier.
    /// </summary>
    public Guid CourierId { get; init; }
}
