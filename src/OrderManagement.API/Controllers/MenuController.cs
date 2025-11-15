using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Menu.Commands.CreateMenuItem;
using OrderManagement.Application.Menu.Commands.DeleteMenuItem;
using OrderManagement.Application.Menu.Commands.UpdateMenuItem;
using OrderManagement.Application.Menu.Queries.GetMenuItems;
using OrderManagement.Contracts.Menu.Requests;
using OrderManagement.Contracts.Menu.Responses;

namespace OrderManagement.API.Controllers;

/// <summary>
/// Menu Controller
/// </summary>
/// <seealso cref="ControllerBase" />
[ApiController]
[Route("api/v1/[controller]")]
public sealed class MenuController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets the menu.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyCollection<MenuItemResponse>>> GetMenu(CancellationToken cancellationToken)
    {
        var query = new GetMenuItemsQuery();

        var result = await mediator.Send(query, cancellationToken);

        var responses = mapper.Map<IReadOnlyCollection<MenuItemResponse>>(result);

        return Ok(responses);
    }

    /// <summary>
    /// Creates the menu item.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<MenuItemResponse>> CreateMenuItem([FromBody] UpsertMenuItemRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateMenuItemCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = mapper.Map<MenuItemResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Updates the menu item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<ActionResult<MenuItemResponse>> UpdateMenuItem(Guid id, [FromBody] UpsertMenuItemRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateMenuItemCommand>(request) with { Id = id };

        var result = await mediator.Send(command, cancellationToken);

        var response = mapper.Map<MenuItemResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Deletes the menu item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> DeleteMenuItem(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteMenuItemCommand(id);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}