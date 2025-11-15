namespace OrderManagement.Contracts.Common.Errors;

/// <summary>
/// Error Response
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the code.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the status code.
    /// </summary>
    public int StatusCode { get; init; }
}
