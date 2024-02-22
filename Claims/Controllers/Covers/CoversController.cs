using Claims.Auditing;
using Claims.Controllers.Covers.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers.Covers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<CoversController> _logger;
    private readonly Auditer _auditer;

    public CoversController(CosmosClient cosmosClient, AuditContext auditContext, ILogger<CoversController> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
        _auditer = new Auditer(auditContext);
    }
    
    [HttpPost("/premium")]
    public ActionResult ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var container = await Container();
        var query = container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Cover>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var container = await Container();
        try
        {
            var response = await container.ReadItemAsync<Cover>(id, new (id));
            return Ok(ToDto(response.Resource));
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateCoverDto cover)
    {
        var container = await Container();

        var id = Guid.NewGuid().ToString();
        var premium = ComputePremium(cover.StartDate, cover.EndDate, cover.CoverType);

        var item = new Cover
        {
            Id = id,
            Type = cover.CoverType,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Premium = premium
        };
        await container.CreateItemAsync(item, new PartitionKey(id));
        _auditer.AuditCover(id, "POST");
        return Ok(ToDto(item));
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        var container = await Container();
        _auditer.AuditCover(id, "DELETE");
        await container.DeleteItemAsync<Cover>(id, new (id));
    }

    private decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var multiplier = 1.3m;
        if (coverType == CoverType.Yacht)
        {
            multiplier = 1.1m;
        }

        if (coverType == CoverType.PassengerShip)
        {
            multiplier = 1.2m;
        }

        if (coverType == CoverType.Tanker)
        {
            multiplier = 1.5m;
        }

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30) totalPremium += premiumPerDay;
            if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
        }

        return totalPremium;
    }

    private CoverDto ToDto(Cover cover)
    {
        return new CoverDto
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            CoverType = cover.Type,
            Premium = cover.Premium,
        };
    }

    private async Task<Container> Container()
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync("ClaimDb");
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync("Cover", "/id");
        var container = containerResponse.Container;
        return container;
    }
}