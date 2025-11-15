namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Open Route Service Geocode Response
/// </summary>
internal sealed record OpenRouteServiceGeocodeResponse
{
    /// <summary>
    /// Gets the features.
    /// </summary>
    public Feature[]? Features { get; init; }
}
