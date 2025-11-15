namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Route Segment
/// </summary>
internal sealed record RouteSegment
{
    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    public double? Duration { get; init; } // seconds
}
