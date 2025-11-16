using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.ValueObjects;
using OrderManagement.Infrastructure.Options;
using OrderManagement.Infrastructure.Services.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Infrastructure.Services;

/// <summary>
/// Open Route Service Eta Service
/// </summary>
/// <seealso cref="IDeliveryEtaService" />
public sealed class OpenRouteServiceEtaService(HttpClient httpClient, IOptions<OpenRouteServiceOptions> options, ILogger<OpenRouteServiceEtaService> logger) : IDeliveryEtaService
{
    private readonly OpenRouteServiceOptions options = options.Value;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private const string _geocodePath = "geocode/search";
    private string DirectionsPath => $"v2/directions/{options.Profile}/geojson?api_key={options.ApiKey}";

    /// <summary>
    /// Calculates the estimated travel time for a delivery order based on its address.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="cancellationToken"></param>
    public async Task<DeliveryEstimate?> GetEstimateAsync(DeliveryAddress address, CancellationToken cancellationToken = default)
    {
        try
        {
            var destination = await GeocodeAsync(address, cancellationToken);

            if (destination is null)
            {
                logger.LogWarning("Failed to geocode delivery address. Using fallback delivery time.");
                return default;
            }

            var durationSeconds = await GetRouteDurationSecondsAsync(
                originLat: options.RestaurantLatitude,
                originLon: options.RestaurantLongitude,
                destLat: destination.Value.Latitude,
                destLon: destination.Value.Longitude,
                cancellationToken
            );

            if (durationSeconds is null || durationSeconds <= 0)
            {
                logger.LogWarning("OpenRouteService returned non-positive duration");
                return default;
            }

            return new DeliveryEstimate(TimeSpan.FromSeconds(durationSeconds.Value));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while calling OpenRouteService");
            return default;
        }
    }

    /// <summary>
    /// Geocodes the delivery address into latitude/longitude using ORS geocoding API.
    /// </summary>
    private async Task<(double Latitude, double Longitude)?> GeocodeAsync(DeliveryAddress address, CancellationToken cancellationToken)
    {
        var query = BuildAddressQuery(address);

        var requestUri = $"{_geocodePath}?api_key={options.ApiKey}&text={Uri.EscapeDataString(query)}&size=1";

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);

        response.EnsureSuccessStatusCode();

        var geo = await response.Content.ReadFromJsonAsync<OpenRouteServiceGeocodeResponse>(Json, cancellationToken);

        if (geo?.Features is null || geo.Features.Length == 0)
        {
            logger.LogWarning("OpenRouteService geocode returned no features for address: {Address}", query);
            return default;
        }

        var coordinates = geo.Features[0]?.Geometry?.Coordinates;

        if (coordinates is null || coordinates.Length < 2)
        {
            logger.LogWarning("OpenRouteService geocode returned invalid coordinates for address: {Address}", query);
            return default;
        }

        var lon = coordinates[0];
        var lat = coordinates[1];

        return (lat, lon);
    }

    /// <summary>
    /// Calls ORS directions API and returns travel duration in seconds.
    /// </summary>
    private async Task<double?> GetRouteDurationSecondsAsync(
        double originLat,
        double originLon,
        double destLat,
        double destLon,
        CancellationToken cancellationToken
    )
    {
        var payload = new OpenRouteServiceDirectionsRequestBuilder()
            .AddWaypoint(originLon, originLat)
            .AddWaypoint(destLon, destLat)
            .Build();

        using var response = await httpClient.PostAsJsonAsync(DirectionsPath, payload, Json, cancellationToken);

        response.EnsureSuccessStatusCode();

        var directions = await response.Content.ReadFromJsonAsync<OpenRouteServiceDirectionsResponse>(Json, cancellationToken);

        if (directions?.Features is null || directions.Features.Length == 0)
        {
            logger.LogWarning("OpenRouteService directions returned no features.");
            return default;
        }

        var segments = directions.Features[0]?.Properties?.Segments;

        if (segments is null || segments.Length == 0)
        {
            logger.LogWarning("OpenRouteService directions returned no segments.");
            return default;
        }

        return segments[0].Duration;
    }

    private static string BuildAddressQuery(DeliveryAddress address)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(address.Street)) sb.Append(address.Street);

        AppendAddress(address.Line2, sb);
        AppendAddress(address.City, sb);
        AppendAddress(address.Zip, sb);
        AppendAddress(address.Country, sb);

        return sb.ToString();
    }

    private static void AppendAddress(string? addressPart, StringBuilder sb)
    {
        if (string.IsNullOrWhiteSpace(addressPart)) return;

        if (sb.Length > 0) sb.Append(", ");

        sb.Append(addressPart);
    }
}