using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers.Claims
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly CosmosDbService _cosmosDbService;
        private readonly Auditer _auditer;

        public ClaimsController(ILogger<ClaimsController> logger, CosmosDbService cosmosDbService, AuditContext auditContext)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
            _auditer = new Auditer(auditContext);
        }

        [HttpGet]
        public async Task<IEnumerable<ClaimDto>> GetAsync()
        {
            var claims = await _cosmosDbService.GetClaimsAsync();
            return claims.Select(ToDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateClaimDto claim)
        {
            var id = Guid.NewGuid();
            var item = new Claim
            {
                Id = id.ToString(),
                Name = claim.Name!,
                Type = claim.ClaimType!.Value,
                CoverId = claim.CoverId!.Value.ToString(),
                Created = claim.Created!.Value,
                DamageCost = claim.DamageCost!.Value
            };
            await _cosmosDbService.AddItemAsync(item);
            _auditer.AuditClaim(item.Id, "POST");
            return Ok(ToDto(item));
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            return _cosmosDbService.DeleteItemAsync(id);
        }

        [HttpGet("{id}")]
        public Task<Claim> GetAsync(string id)
        {
            return _cosmosDbService.GetClaimAsync(id);
        }

        private static ClaimDto ToDto(Claim item)
        {
            return new ClaimDto
            {
                Id = Guid.Parse(item.Id),
                CoverId = Guid.Parse(item.CoverId),
                ClaimType = item.Type,
                Created = item.Created,
                DamageCost = item.DamageCost,
                Name = item.Name
            };
        }
    }
}