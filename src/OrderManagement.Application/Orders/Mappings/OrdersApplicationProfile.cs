using AutoMapper;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.ValueObjects;

namespace OrderManagement.Application.Orders.Mappings;

/// <summary>
/// Orders Application Profile
/// </summary>
/// <seealso cref="Profile" />
public sealed class OrdersApplicationProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersApplicationProfile"/> class.
    /// </summary>
    public OrdersApplicationProfile()
    {
        // Domain → Application model
        CreateMap<Order, OrderModel>()
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));

        CreateMap<OrderItem, OrderItemModel>()
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));

        CreateMap<DeliveryAddress, DeliveryAddressModel>();

        CreateMap<AssignmentInfo, AssignmentInfoModel>();
    }
}
