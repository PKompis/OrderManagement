using AutoMapper;
using OrderManagement.Application.Admin.Queries;
using OrderManagement.Contracts.Admin.Responses;

namespace OrderManagement.API.Mappings;

/// <summary>
/// Admin Api Profile
/// </summary>
/// <seealso cref="Profile" />
public sealed class AdminApiProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminApiProfile"/> class.
    /// </summary>
    public AdminApiProfile()
    {
        //Aplication Models/Result -> Response DTO
        CreateMap<OrderStatisticsResult, StatisticsResponse>();
    }
}
