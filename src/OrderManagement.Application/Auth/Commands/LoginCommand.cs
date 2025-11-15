using MediatR;

namespace OrderManagement.Application.Auth.Commands;

public sealed record LoginCommand(Guid UserId, bool IsStaff) : IRequest<LoginResult>;