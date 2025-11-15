using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Admin.Queries;
using OrderManagement.Contracts.Admin.Responses;

namespace OrderManagement.API.Controllers;

/// <summary>
/// Admin Controller
/// </summary>
/// <seealso cref="ControllerBase" />
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AdminController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets the order statistics.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet("statistics/orders")]
    public async Task<ActionResult<StatisticsResponse>> GetOrderStatistics(CancellationToken cancellationToken)
    {
        var query = new GetOrderStatisticsQuery();

        var result = await mediator.Send(query, cancellationToken);

        var response = mapper.Map<StatisticsResponse>(result);

        return Ok(response);
    }
}
