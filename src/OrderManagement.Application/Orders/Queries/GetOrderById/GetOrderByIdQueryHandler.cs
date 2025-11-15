using AutoMapper;
using MediatR;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Get Order ById Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetOrderByIdQuery, OrderResult}" />
public sealed class GetOrderByIdQueryHandler(IOrderReadRepository readRepository, IMapper mapper) : IRequestHandler<GetOrderByIdQuery, OrderResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="NotFoundException">Order</exception>
    public async Task<OrderResult> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await readRepository.GetByIdAsync(request.OrderId, cancellationToken) ?? throw new NotFoundException("Order", request.OrderId);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }
}
