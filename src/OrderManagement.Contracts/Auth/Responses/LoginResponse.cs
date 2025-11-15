namespace OrderManagement.Contracts.Auth.Responses;

/// <summary>
/// Login Response
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// The JWT access token.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// When the token expires (UTC).
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// The application role associated with this token (Customer, Kitchen, Delivery, Admin).
    /// </summary>
    public string Role { get; init; } = string.Empty;
}
