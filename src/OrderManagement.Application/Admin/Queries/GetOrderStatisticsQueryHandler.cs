using AutoMapper;
using MediatR;
using OrderManagement.Application.Orders.Abstractions;

namespace OrderManagement.Application.Admin.Queries;

/// <summary>
/// Get Order Statistics Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetOrderStatisticsQuery, OrderStatisticsResult}" />
public sealed class GetOrderStatisticsQueryHandler(IOrderReadRepository orderReadRepository, IMapper mapper) : IRequestHandler<GetOrderStatisticsQuery, OrderStatisticsResult>
{
    public async Task<OrderStatisticsResult> Handle(GetOrderStatisticsQuery request, CancellationToken cancellationToken)
    {
        var stats = await orderReadRepository.GetStatisticsAsync(cancellationToken);

        return mapper.Map<OrderStatisticsResult>(stats);
    }
}