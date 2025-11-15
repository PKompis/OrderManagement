using AutoMapper;
using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Customers.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Orders.ValueObjects;

namespace OrderManagement.Application.Orders.Commands.PlaceOrder;

public sealed class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IMenuItemReadRepository menuItemReadRepository,
    ICustomerReadRepository customerReadRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<PlaceOrderCommand, OrderResult>
{
    public async Task<OrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var customerExists = await customerReadRepository.ExistsAsync(request.CustomerId, cancellationToken);

        if (!customerExists) throw new NotFoundException("Customer", request.CustomerId);

        var orderItems = await RetrieveOrderItems(request, cancellationToken);

        var deliveryAddress = request.Type == OrderType.Delivery ? RetrieveDeliveryAddress(request) : default;

        var order = Order.Create(
            customerId: request.CustomerId,
            type: request.Type,
            items: orderItems,
            deliveryAddress: deliveryAddress
        );

        await orderRepository.AddAsync(order, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }

    private async Task<List<OrderItem>> RetrieveOrderItems(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var menuItem = await menuItemReadRepository.GetByIdAsync(item.MenuItemId, cancellationToken) ?? throw new NotFoundException("Menu item", item.MenuItemId);

            var orderItem = OrderItem.Create
            (
                menuItemId: menuItem.Id,
                name: menuItem.Name,
                unitPrice: menuItem.Price,
                quantity: item.Quantity,
                notes: item.Notes
            );

            orderItems.Add(orderItem);
        }

        return orderItems;
    }

    private static DeliveryAddress? RetrieveDeliveryAddress(PlaceOrderCommand request)
    {
        var addr = request.DeliveryAddress!;

        var deliveryAddress = DeliveryAddress.Create(
            street: addr.Street,
            city: addr.City,
            zip: addr.Zip,
            line2: addr.Line2,
            country: addr.Country
        );

        return deliveryAddress;
    }
}
