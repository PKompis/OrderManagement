namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Geometry
/// </summary>
internal sealed record Geometry
{
    /// <summary>
    /// Gets or sets the coordinates.
    /// </summary>
    public double[]? Coordinates { get; init; }
}
