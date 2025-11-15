namespace OrderManagement.Application.Common.Models;

/// <summary>
/// Token Result
/// </summary>
public sealed record TokenResult(string AccessToken, DateTimeOffset ExpiresAt, string Role);
