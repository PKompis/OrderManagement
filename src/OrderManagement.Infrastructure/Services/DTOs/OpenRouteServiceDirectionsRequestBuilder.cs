namespace OrderManagement.Infrastructure.Services.DTOs;

/// <summary>
/// Open Route Service Directions Request Builder
/// </summary>
internal sealed class OpenRouteServiceDirectionsRequestBuilder
{
    private readonly List<double[]> _coordinates = [];

    /// <summary>
    /// Adds the waypoint.
    /// </summary>
    /// <param name="lon">The lon.</param>
    /// <param name="lat">The lat.</param>
    public OpenRouteServiceDirectionsRequestBuilder AddWaypoint(double lon, double lat)
    {
        _coordinates.Add([lon, lat]);
        return this;
    }

    public OpenRouteServiceDirectionsRequest Build()
    {
        if (_coordinates.Count < 2) throw new InvalidOperationException("Directions request must contain at least two coordinates (origin and destination).");

        return new OpenRouteServiceDirectionsRequest
        {
            Coordinates = [.. _coordinates]
        };
    }
}
