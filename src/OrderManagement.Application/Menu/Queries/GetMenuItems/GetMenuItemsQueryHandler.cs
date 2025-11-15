using AutoMapper;
using MediatR;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Results;

namespace OrderManagement.Application.Menu.Queries.GetMenuItems;

/// <summary>
/// Get Menu Items Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetMenuItemsQuery, MenuItemsResult}" />
public sealed class GetMenuItemsQueryHandler(IMenuItemReadRepository readRepository, IMapper mapper) : IRequestHandler<GetMenuItemsQuery, MenuItemsResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    public async Task<MenuItemsResult> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await readRepository.GetAllAsync(cancellationToken);

        var models = mapper.Map<IReadOnlyCollection<MenuItemModel>>(items);

        return new MenuItemsResult { Items = models };
    }
}