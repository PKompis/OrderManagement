using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Application.Admin.Queries;

/// <summary>
/// Get Order Statistics Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetOrderStatisticsQuery, OrderStatisticsResult}" />
public sealed class GetOrderStatisticsQueryHandler(IOrderReadRepository orderReadRepository, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetOrderStatisticsQuery, OrderStatisticsResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    public async Task<OrderStatisticsResult> Handle(GetOrderStatisticsQuery request, CancellationToken cancellationToken)
    {
        ValidateCurrentUser();

        var stats = await orderReadRepository.GetStatisticsAsync(cancellationToken);

        return mapper.Map<OrderStatisticsResult>(stats);
    }

    private void ValidateCurrentUser()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to view statistics.");

        if (!string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("Only admin can view statistics.");
    }
}