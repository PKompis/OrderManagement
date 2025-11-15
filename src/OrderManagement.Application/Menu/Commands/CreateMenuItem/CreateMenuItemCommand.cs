using MediatR;
using OrderManagement.Application.Menu.Results;

namespace OrderManagement.Application.Menu.Commands.CreateMenuItem;

/// <summary>
/// Create Menu Item Command
/// </summary>
/// <seealso cref="IRequest{MenuItemResult}" />
public sealed record CreateMenuItemCommand(
    string Name,
    decimal Price,
    string Category,
    bool IsAvailable
) : IRequest<MenuItemResult>;
