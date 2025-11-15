namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Route Properties
/// </summary>
internal sealed record RouteProperties
{
    /// <summary>
    /// Gets or sets the segments.
    /// </summary>
    public RouteSegment[]? Segments { get; init; }
}
