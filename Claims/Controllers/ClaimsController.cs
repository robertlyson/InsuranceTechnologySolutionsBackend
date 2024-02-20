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
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
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