using Claims.Controllers.Dto;
using Infrastructure.Claims;
using Infrastructure.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClaimsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CollectionResponse<ClaimDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<CollectionResponse<ClaimDto>> GetAsync(int take = 10, int skip = 0, string? name = null,
        CancellationToken cancellationToken = default)
    {
        var claims = await _mediator.Send(new GetClaimsQuery(take, skip, name), cancellationToken);
        return new CollectionResponse<ClaimDto>(claims);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), 200)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimDto claim, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new CreateClaimCommand(claim), cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error.Description);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(NotFoundResult), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new DeleteClaimCommand(id), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimDto), 200)]
    [ProducesResponseType(typeof(NotFoundResult), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var claimDto = await _mediator.Send(new GetClaimQuery(id), cancellationToken);
        return claimDto == null ? NotFound() : Ok(claimDto);
    }
}