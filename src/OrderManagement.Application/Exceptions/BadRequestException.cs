using OrderManagement.Domain.Common.Exceptions;
using System.Net;

namespace OrderManagement.Application.Exceptions;

/// <summary>
/// Bad request exception
/// </summary>
/// <seealso cref="BaseException" />
public sealed class BadRequestException(string message) : BaseException(message, statusCode: HttpStatusCode.BadRequest, errorCode: "bad_request") {}
