using OrderManagement.Domain.Common.Exceptions;
using System.Net;

namespace OrderManagement.Application.Exceptions;

/// <summary>
/// Validation Exception
/// </summary>
/// <seealso cref="BaseException" />
public sealed class ValidationFailedException(IEnumerable<string> errors) : BaseException("Validation failed.", HttpStatusCode.BadRequest, "validation_failed")
{
    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; } = errors.ToList().AsReadOnly();
}
