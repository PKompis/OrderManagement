using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Customers.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.Application.Auth.Commands;

/// <summary>
/// Login Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{LoginCommand, LoginResult}" />
public sealed class LoginCommandHandler(ICustomerReadRepository customerReadRepository, IStaffReadRepository staffReadRepository, ITokenService tokenService) : IRequestHandler<LoginCommand, LoginResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Staff member not found.
    /// or
    /// Staff member is inactive.
    /// or
    /// Customer not found.
    /// </exception>
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        string role;
        Guid userId;

        if (request.IsStaff)
        {
            var staff = await staffReadRepository.GetByIdAsync(request.UserId, cancellationToken) ?? throw new BadRequestException("Staff member not found.");
            if (!staff.IsActive) throw new BadRequestException("Staff member is inactive.");

            role = staff.Role switch
            {
                StaffRole.Kitchen => ApplicationRoles.Kitchen,
                StaffRole.Delivery => ApplicationRoles.Delivery,
                StaffRole.Admin => ApplicationRoles.Admin,
                _ => "Undefined"
            };

            userId = staff.Id;
        }
        else
        {
            var exists = await customerReadRepository.ExistsAsync(request.UserId, cancellationToken);
            if (!exists) throw new BadRequestException("Customer not found.");

            role = ApplicationRoles.Customer;
            userId = request.UserId;
        }

        var token = tokenService.GenerateToken(userId, role);

        return new LoginResult(
            AccessToken: token.AccessToken,
            ExpiresAt: token.ExpiresAt,
            Role: token.Role
        );
    }
}