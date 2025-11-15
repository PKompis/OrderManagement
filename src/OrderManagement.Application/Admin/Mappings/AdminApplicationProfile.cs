using AutoMapper;
using OrderManagement.Application.Admin.Queries;
using OrderManagement.Application.Orders.Models;

namespace OrderManagement.Application.Admin.Mappings;

/// <summary>
/// Admun Application Profile
/// </summary>
/// <seealso cref="Profile" />
public sealed class AdminApplicationProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminApplicationProfile"/> class.
    /// </summary>
    public AdminApplicationProfile()
    {
        // Application Model → Application Result
        CreateMap<OrderStatisticsModel, OrderStatisticsResult>();
    }
}
