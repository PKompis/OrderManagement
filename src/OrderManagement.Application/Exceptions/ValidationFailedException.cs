using OrderManagement.Domain.Common.Exceptions;
using System.Net;

namespace OrderManagement.Application.Exceptions;

/// <summary>
/// Validation Exception
/// </summary>
/// <seealso cref="BaseException" />
public sealed class ValidationFailedException(IEnumerable<string> errors) : BaseException($"Validation failed: {string.Join(',', errors)}", HttpStatusCode.BadRequest, "validation_failed") { }
