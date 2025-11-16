using System.Net;

namespace OrderManagement.Domain.Common.Exceptions;

/// <summary>
/// Represents an authorization failure (HTTP 403 Forbidden).
/// Thrown when the user is authenticated but is not allowed to perform the action.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ForbiddenException"/> class.
/// </remarks>
/// <param name="message">The message describing the error.</param>
/// <param name="errorCode">The optional error code.</param>
public sealed class ForbiddenException(string message = "You do not have permission to perform this action.", string errorCode = "Error.Forbidden") : BaseException(message, HttpStatusCode.Forbidden, errorCode) { }