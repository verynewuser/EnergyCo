using EnergyCo.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnergyCo.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PointsPromotionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PointsPromotionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _mediator.Send(new GetPointsPromotionsQuery()));            
    }
}