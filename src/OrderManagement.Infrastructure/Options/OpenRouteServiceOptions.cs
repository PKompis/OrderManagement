namespace OrderManagement.Infrastructure.Options;

public sealed class OpenRouteServiceOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "OpenRouteService";

    /// <summary>
    /// Gets the API key.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Base API URL, defaults to official ORS endpoint.
    /// </summary>
    public string? BaseUrl { get; init; }

    /// <summary>
    /// Routing profile (e.g. driving-car, cycling-regular).
    /// </summary>
    public string? Profile { get; init; }

    /// <summary>
    /// Restaurant latitude.
    /// </summary>
    public double RestaurantLatitude { get; init; }

    /// <summary>
    /// Restaurant longitude.
    /// </summary>
    public double RestaurantLongitude { get; init; }
}
