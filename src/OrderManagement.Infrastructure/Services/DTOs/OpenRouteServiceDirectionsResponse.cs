namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Open Route Service Directions Response
/// </summary>
internal sealed record OpenRouteServiceDirectionsResponse
{
    /// <summary>
    /// Gets or sets the features.
    /// </summary>
    public RouteFeature[]? Features { get; init; }
}
