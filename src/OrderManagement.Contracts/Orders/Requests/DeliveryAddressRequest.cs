namespace OrderManagement.Contracts.Orders.Requests;

/// <summary>
/// Delivery Address Request
/// </summary>
public sealed record DeliveryAddressRequest
{
    /// <summary>
    /// Gets the street.
    /// </summary>
    public string Street { get; init; } = string.Empty;

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Gets the zip.
    /// </summary>
    public string Zip { get; init; } = string.Empty;

    /// <summary>
    /// Gets the line2.
    /// </summary>
    public string? Line2 { get; init; }

    /// <summary>
    /// Gets the country.
    /// </summary>
    public string? Country { get; init; }
}
