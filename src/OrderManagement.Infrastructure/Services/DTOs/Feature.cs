namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Feature
/// </summary>
internal sealed record Feature
{
    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    public Geometry? Geometry { get; init; }
}
