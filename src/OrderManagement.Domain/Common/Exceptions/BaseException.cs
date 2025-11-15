namespace OrderManagement.Domain.Common.Exceptions;

using System;
using System.Net;

/// <summary>
/// Base exception type for all domain and application errors.
/// </summary>
public abstract class BaseException : Exception
{
    /// <summary>
    /// Gets the associated HTTP status code for this error.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets a stable, machine-readable error code.
    /// Defaults to the class name if not provided.
    /// </summary>
    public string ErrorCode { get; }

    protected BaseException(string message, HttpStatusCode statusCode, string? errorCode = null) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode ?? GetType().Name;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="String" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"{ErrorCode} ({(int)StatusCode}): {Message}";
}
