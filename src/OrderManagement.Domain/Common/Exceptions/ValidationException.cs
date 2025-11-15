using System.Net;

namespace OrderManagement.Domain.Common.Exceptions;

/// <summary>
/// Validation Exception
/// </summary>
/// <seealso cref="BaseException" />
public sealed class ValidationException(string message, string? errorCode = null) : BaseException(message, HttpStatusCode.BadRequest, errorCode) { }