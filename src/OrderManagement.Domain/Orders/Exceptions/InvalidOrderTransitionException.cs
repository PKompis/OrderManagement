namespace OrderManagement.Domain.Orders.Exceptions;

using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Enums;
using System.Net;

/// <summary>
/// Invalid Order Transition Exception
/// </summary>
/// <seealso cref="BaseException" />
public sealed class InvalidOrderTransitionException(OrderType type, OrderStatus from, OrderStatus to) :
    BaseException($"Invalid transition for {type}: {from} → {to}", HttpStatusCode.Conflict)
{ }
