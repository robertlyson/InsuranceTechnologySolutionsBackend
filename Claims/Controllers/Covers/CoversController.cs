using Claims.Application.Covers;
using Claims.Application.Covers.Dto;
using Claims.Auditing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers.Covers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPremiumStrategy _premiumStrategy;
    private readonly Auditer _auditer;

    public CoversController(IMediator mediator,
        IPremiumStrategy premiumStrategy,
        AuditContext auditContext)
    {
        _mediator = mediator;
        _premiumStrategy = premiumStrategy;
        _auditer = new Auditer(auditContext);
    }
    
    [HttpPost("/premium")]
    public ActionResult ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_premiumStrategy.Calculate(startDate, endDate, coverType));
    }

    [HttpGet]
    public Task<IEnumerable<CoverDto>> GetAsync(int take = 10, int skip = 0, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new GetCoversQuery(take, skip), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var cover = await _mediator.Send(new GetCoverQuery(id), cancellationToken);

        return cover == null ? NotFound() : Ok(cover);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateCoverDto cover, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new CreateCoverCommand(cover), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error.Description);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new DeleteCoverCommand(id), cancellationToken);
    }
}