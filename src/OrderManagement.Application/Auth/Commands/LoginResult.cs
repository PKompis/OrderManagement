namespace OrderManagement.Application.Auth.Commands;

/// <summary>
/// Login Result
/// </summary>
public sealed record LoginResult(string AccessToken, DateTimeOffset ExpiresAt, string Role);
