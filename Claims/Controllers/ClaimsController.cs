using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
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
        public Task<IEnumerable<Claim>> GetAsync()
        {
            return _cosmosDbService.GetClaimsAsync();
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
            return Ok(new ClaimDto
            {
                Id = id,
                CoverId = claim.CoverId!.Value,
                ClaimType = claim.ClaimType!.Value,
                Created = claim.Created!.Value,
                DamageCost = claim.DamageCost!.Value,
                Name = claim.Name!
            });
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
    }
}