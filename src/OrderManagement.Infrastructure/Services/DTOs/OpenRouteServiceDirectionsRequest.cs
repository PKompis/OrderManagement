namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Open Route Service Directions Request
/// </summary>
internal sealed record OpenRouteServiceDirectionsRequest
{
    /// <summary>
    /// Gets or sets the coordinates.
    /// </summary>
    public double[][]? Coordinates { get; init; }
}
