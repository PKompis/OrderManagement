using OrderManagement.Domain.Common.Exceptions;
using System.Net;

namespace OrderManagement.Application.Exceptions;

/// <summary>
/// Not Found Exception
/// </summary>
/// <seealso cref="BaseException" />
public sealed class NotFoundException(string resource, object key) : BaseException($"{resource} '{key}' was not found.", statusCode: HttpStatusCode.NotFound, errorCode: "not_found") { }
