using AutoMapper;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Application.Menu.Mappings;

/// <summary>
/// Menu Application Profile
/// </summary>
/// <seealso cref="Profile" />
public sealed class MenuApplicationProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuApplicationProfile"/> class.
    /// </summary>
    public MenuApplicationProfile()
    {
        // Domain → Application model
        CreateMap<MenuItem, MenuItemModel>();
    }
}
