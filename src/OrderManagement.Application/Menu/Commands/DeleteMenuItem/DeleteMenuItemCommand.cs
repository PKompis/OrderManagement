using MediatR;

namespace OrderManagement.Application.Menu.Commands.DeleteMenuItem;

/// <summary>
/// Delete Menu Item Command
/// </summary>
/// <seealso cref="IRequest" />
public sealed record DeleteMenuItemCommand(Guid Id) : IRequest;
