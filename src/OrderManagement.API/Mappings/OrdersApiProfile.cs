using AutoMapper;
using OrderManagement.Application.Orders.Commands.AssignOrder;
using OrderManagement.Application.Orders.Commands.PlaceOrder;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Contracts.Orders.Enums;
using OrderManagement.Contracts.Orders.Requests;
using OrderManagement.Contracts.Orders.Responses;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.API.Mappings;

/// <summary>
/// Mapping Related Profiles for Orders
/// </summary>
/// <seealso cref="Profile" />
public sealed class OrdersApiProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersApiProfile"/> class.
    /// </summary>
    public OrdersApiProfile()
    {
        //Enums DTO -> Domain
        CreateMap<OrderTypeDto, OrderType>().ConvertUsing(src => (OrderType)src);
        CreateMap<OrderStatusDto, OrderStatus>().ConvertUsing(src => (OrderStatus)src);

        //Enums Domain -> DTO
        CreateMap<OrderType, OrderTypeDto>().ConvertUsing(src => (OrderTypeDto)src);
        CreateMap<OrderStatus, OrderStatusDto>().ConvertUsing(src => (OrderStatusDto)src);

        //Request DTO -> Application Commands / Models
        CreateMap<PlaceOrderRequest, PlaceOrderCommand>();
        //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
        //.ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
        //.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        CreateMap<DeliveryAddressRequest, DeliveryAddressModel>();
        CreateMap<PlaceOrderItemRequest, PlaceOrderItemModel>();
        CreateMap<AssignOrderRequest, AssignOrderCommand>();

        //Application Models / Results -> Response DTO
        CreateMap<OrderModel, OrderResponse>();
        //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
        //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        //.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
        //.ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
        //.ForMember(dest => dest.Assignment, opt => opt.MapFrom(src => src.Assignment));
        CreateMap<OrderItemModel, OrderItemResponse>();
        CreateMap<DeliveryAddressModel, DeliveryAddressResponse>();
        CreateMap<AssignmentInfoModel, AssignmentInfoResponse>();
        CreateMap<OrderResult, OrderResponse>().ConvertUsing((src, _, ctx) => ctx.Mapper.Map<OrderResponse>(src.Order));
        CreateMap<OrdersResult, IReadOnlyCollection<OrderResponse>>().ConvertUsing((src, _, ctx) => [.. src.Orders.Select(o => ctx.Mapper.Map<OrderResponse>(o))]);
    }
}
