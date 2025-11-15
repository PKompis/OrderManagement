namespace OrderManagement.API.Options;

/// <summary>
/// Jwt Options
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets the issuer.
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Gets the audience.
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Gets the secret key.
    /// </summary>
    public string SecretKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets the expiration minutes.
    /// </summary>
    public int ExpirationMinutes { get; init; } = 60;
}
