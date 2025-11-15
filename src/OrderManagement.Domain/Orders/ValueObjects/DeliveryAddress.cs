using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Domain.Orders.ValueObjects;

/// <summary>
/// Address for delivery orders
/// </summary>
public sealed record DeliveryAddress
{
    /// <summary>
    /// Gets the street.
    /// </summary>
    public string Street { get; init; } = string.Empty;

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Gets the zip.
    /// </summary>
    public string Zip { get; init; } = string.Empty;

    /// <summary>
    /// Gets the line2.
    /// </summary>
    public string? Line2 { get; init; }

    /// <summary>
    /// Gets the country.
    /// </summary>
    public string? Country { get; init; }

    private DeliveryAddress() { }

    private DeliveryAddress(string street, string city, string zip, string? line2, string? country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ValidationException("Street is required.", "DeliveryAddress.StreetRequired");
        if (string.IsNullOrWhiteSpace(city)) throw new ValidationException("City is required.", "DeliveryAddress.CityRequired");
        if (string.IsNullOrWhiteSpace(zip)) throw new ValidationException("ZIP/Postal code is required.", "DeliveryAddress.ZipRequired");

        Street = street.Trim();
        City = city.Trim();
        Zip = zip.Trim();
        Line2 = string.IsNullOrWhiteSpace(line2) ? null : line2.Trim();
        Country = string.IsNullOrWhiteSpace(country) ? null : country.Trim();
    }

    /// <summary>
    /// Creates the specified street.
    /// </summary>
    /// <param name="street">The street.</param>
    /// <param name="city">The city.</param>
    /// <param name="zip">The zip.</param>
    /// <param name="line2">The line2.</param>
    /// <param name="country">The country.</param>
    public static DeliveryAddress Create(string street, string city, string zip, string? line2 = null, string? country = null)
        => new(street, city, zip, line2, country);
}