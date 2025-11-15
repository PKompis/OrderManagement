namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Route Feature
/// </summary>
internal sealed record RouteFeature
{
    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    public RouteProperties? Properties { get; init; }
}
