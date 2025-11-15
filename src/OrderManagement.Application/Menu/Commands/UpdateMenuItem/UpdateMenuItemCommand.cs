using MediatR;
using OrderManagement.Application.Menu.Results;

namespace OrderManagement.Application.Menu.Commands.UpdateMenuItem;

/// <summary>
/// Update Menu Item Command
/// </summary>
/// <seealso cref="IRequest{MenuItemResult}" />
public sealed record UpdateMenuItemCommand(
    Guid Id,
    string Name,
    decimal Price,
    string Category,
    bool IsAvailable
) : IRequest<MenuItemResult>;
