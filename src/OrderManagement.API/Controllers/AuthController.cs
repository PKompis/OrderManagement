using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Auth.Commands;
using OrderManagement.Contracts.Auth.Requests;
using OrderManagement.Contracts.Auth.Responses;

namespace OrderManagement.API.Controllers;

/// <summary>
/// Authentication Controller – issues JWT tokens for customers and staff.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Issues a JWT for a mock customer or staff member.
    /// Authentication is simplified to an ID + IsStaff flag.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.UserId, request.IsStaff);

        var result = await mediator.Send(command, cancellationToken);

        var response = new LoginResponse
        {
            AccessToken = result.AccessToken,
            ExpiresAt = result.ExpiresAt,
            Role = result.Role
        };

        return Ok(response);
    }
}