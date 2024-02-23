using Claims.Application.Covers;
using Claims.Auditing;
using Claims.Controllers.Covers.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers.Covers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<CoversController> _logger;
    private readonly Auditer _auditer;

    public CoversController(IMediator mediator, CosmosClient cosmosClient, AuditContext auditContext, ILogger<CoversController> logger)
    {
        _mediator = mediator;
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
    public Task<IEnumerable<CoverDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new GetCoversQuery(), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
    {
        var container = await Container();
        try
        {
            var response = await container.ReadItemAsync<CoverCosmosEntity>(id, new (id));
            return Ok(Mappers.ToDto(response.Resource));
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
        var premium = ComputePremium(cover.StartDate!.Value, cover.EndDate!.Value, cover.CoverType!.Value);

        var item = new CoverCosmosEntity
        {
            Id = id,
            Type = cover.CoverType.Value,
            StartDate = cover.StartDate.Value,
            EndDate = cover.EndDate.Value,
            Premium = premium
        };
        await container.CreateItemAsync(item, new PartitionKey(id));
        _auditer.AuditCover(id, "POST");
        return Ok(Mappers.ToDto(item));
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        var container = await Container();
        _auditer.AuditCover(id, "DELETE");
        await container.DeleteItemAsync<CoverCosmosEntity>(id, new (id));
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

    private async Task<Container> Container()
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync("ClaimDb");
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync("Cover", "/id");
        var container = containerResponse.Container;
        return container;
    }
}