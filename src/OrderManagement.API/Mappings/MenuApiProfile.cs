using AutoMapper;
using OrderManagement.Application.Menu.Commands.CreateMenuItem;
using OrderManagement.Application.Menu.Commands.UpdateMenuItem;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Results;
using OrderManagement.Contracts.Menu.Requests;
using OrderManagement.Contracts.Menu.Responses;

namespace OrderManagement.API.Mappings;

public sealed class MenuApiProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuApiProfile"/> class.
    /// </summary>
    public MenuApiProfile()
    {
        // Request DTO -> Application Commands
        CreateMap<UpsertMenuItemRequest, CreateMenuItemCommand>();
        CreateMap<UpsertMenuItemRequest, UpdateMenuItemCommand>();

        // Application Models / Results -> Response DTO
        CreateMap<MenuItemModel, MenuItemResponse>();
        CreateMap<MenuItemResult, MenuItemResponse>().ConvertUsing((src, _, ctx) => ctx.Mapper.Map<MenuItemResponse>(src.Item));
        CreateMap<MenuItemsResult, IReadOnlyCollection<MenuItemResponse>>().ConvertUsing((src, _, ctx) => [.. src.Items.Select(i => ctx.Mapper.Map<MenuItemResponse>(i))]);
    }
}
