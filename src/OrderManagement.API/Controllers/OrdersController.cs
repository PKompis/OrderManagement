using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Commands.AssignOrder;
using OrderManagement.Application.Orders.Commands.PlaceOrder;
using OrderManagement.Application.Orders.Commands.UpdateOrderStatus;
using OrderManagement.Application.Orders.Queries.GetDeliveryAssignments;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Application.Orders.Queries.GetOrders;
using OrderManagement.Contracts.Orders.Enums;
using OrderManagement.Contracts.Orders.Requests;
using OrderManagement.Contracts.Orders.Responses;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.API.Controllers;

/// <summary>
/// Orders Controller
/// </summary>
/// <seealso cref="ControllerBase" />
[ApiController]
[Route("api/v1/[controller]")]
public sealed class OrdersController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Places the order.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPost]
    [Authorize(Roles = ApplicationRoles.Customer)]
    public async Task<ActionResult<OrderResponse>> PlaceOrder([FromBody] PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<PlaceOrderCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = mapper.Map<OrderResponse>(result);

        return CreatedAtAction(nameof(GetOrderById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Gets the order by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);

        var result = await mediator.Send(query, cancellationToken);

        var response = mapper.Map<OrderResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Gets the orders.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="type">The type.</param>
    /// <param name="assignedCourierId">The assigned courier identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetOrders(
        [FromQuery] OrderStatusDto? status,
        [FromQuery] OrderTypeDto? type,
        [FromQuery] Guid? assignedCourierId,
        [FromQuery] Guid? customerId,
        CancellationToken cancellationToken
    )
    {
        var query = new GetOrdersQuery
        {
            Status = status.HasValue ? (OrderStatus?)status.Value : null,
            Type = type.HasValue ? (OrderType?)type.Value : null,
            AssignedCourierId = assignedCourierId,
            CustomerId = customerId
        };

        var result = await mediator.Send(query, cancellationToken);

        var responses = mapper.Map<IReadOnlyCollection<OrderResponse>>(result);

        return Ok(responses);
    }

    /// <summary>
    /// Updates the status.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Kitchen}, {ApplicationRoles.Delivery}")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(Guid id, [FromQuery] OrderStatusDto status, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderStatusCommand(id, (OrderStatus)status);

        var result = await mediator.Send(command, cancellationToken);

        var response = mapper.Map<OrderResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Assigns the order.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPost("{id:guid}/assignments")]
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Kitchen}")]
    public async Task<ActionResult<OrderResponse>> AssignOrder(Guid id, [FromBody] AssignOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignOrderCommand(id, request.CourierId);

        var result = await mediator.Send(command, cancellationToken);

        var response = mapper.Map<OrderResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Gets the assignments.
    /// </summary>
    /// <param name="courierId">The courier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet("assignments")]
    [Authorize(Roles = ApplicationRoles.Delivery)]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetAssignments(CancellationToken cancellationToken)
    {
        var query = new GetDeliveryAssignmentsQuery();

        var result = await mediator.Send(query, cancellationToken);

        var responses = mapper.Map<IReadOnlyCollection<OrderResponse>>(result);

        return Ok(responses);
    }
}
