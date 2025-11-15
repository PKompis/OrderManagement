using MediatR;
using OrderManagement.Application.Menu.Results;

namespace OrderManagement.Application.Menu.Queries.GetMenuItems;

/// <summary>
/// Get Menu Items Query
/// </summary>
/// <seealso cref="IRequest{MenuItemsResult}" />
public sealed record GetMenuItemsQuery : IRequest<MenuItemsResult>;
