using Claims.Application.Claims;
using Claims.Application.Claims.Dto;
using Claims.Auditing;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers.Claims
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CosmosDbService _cosmosDbService;
        private readonly Auditer _auditer;

        public ClaimsController(IMediator mediator, CosmosDbService cosmosDbService, AuditContext auditContext)
        {
            _mediator = mediator;
            _cosmosDbService = cosmosDbService;
            _auditer = new Auditer(auditContext);
        }

        [HttpGet]
        public Task<ClaimDto[]> GetAsync(CancellationToken cancellationToken = default)
        {
            return _mediator.Send(new GetClaimsQuery(), cancellationToken);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateClaimDto claim, CancellationToken cancellationToken = default)
        {
            var claimDto = await _mediator.Send(new CreateClaimCommand(claim), cancellationToken);
            return Ok(claimDto);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(new DeleteClaimCommand(id), cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<ClaimDto> GetAsync(string id)
        {
            var claim = await _cosmosDbService.GetClaimAsync(id);
            return Mappers.ToDto(claim);
        }
    }
}