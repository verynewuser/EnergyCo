using EnergyCo.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnergyCo.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DiscountCalculatorController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscountCalculatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CalculationDiscount(CalculateDiscountCommand request)
    {
        return Ok(await _mediator.Send(request));            
    }
}