using Claims.Application.Claims;
using Claims.Application.Covers;
using Claims.Auditing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Claims.Infrastructure;

public class AuditHostedService : IHostedService
{
    private readonly CosmosClient _cosmosClient;
    private readonly IOptions<CosmosDbOption> _options;
    private readonly IServiceProvider _provider;
    private readonly List<ChangeFeedProcessor> _processors = new();

    public AuditHostedService(CosmosClient cosmosClient, IOptions<CosmosDbOption> options, IServiceProvider provider)
    {
        _cosmosClient = cosmosClient;
        _options = options;
        _provider = provider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var databaseName = _options.Value.DatabaseName;
        var claimsContainerName = _options.Value.ClaimsContainerName;
        var coversContainerName = _options.Value.CoversContainerName;
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName, cancellationToken: cancellationToken);
        var database = databaseResponse.Database;

        var claimsProcessor = await BuildProcessor<ClaimCosmosEntity>(database, claimsContainerName,
            HandleClaimChanges, cancellationToken);
        _processors.Add(claimsProcessor);
        var coversProcessor = await BuildProcessor<CoverCosmosEntity>(database, coversContainerName,
            HandleCoverChanges, cancellationToken);
        _processors.Add(coversProcessor);

        foreach (var feedProcessor in _processors)
        {
            await feedProcessor.StartAsync();
        }
    }

    private async Task<ChangeFeedProcessor> BuildProcessor<T>(Database database, string? containerName, Container.ChangesHandler<T> changesHandler, CancellationToken cancellationToken)
    {
        var sourceContainer = await database.CreateContainerIfNotExistsAsync(containerName, "/id", cancellationToken: cancellationToken);
        var leaseContainer = await database.CreateContainerIfNotExistsAsync($"{containerName}_lease", "/id", cancellationToken: cancellationToken);

        var builder = sourceContainer.Container.GetChangeFeedProcessorBuilder(
            processorName: $"{containerName}-processor",
            onChangesDelegate: changesHandler
        );
    
        return builder
            .WithInstanceName("claimsApp")
            .WithLeaseContainer(leaseContainer)
            .Build();
    }

    private async Task HandleClaimChanges(IReadOnlyCollection<ClaimCosmosEntity> changes, CancellationToken token)
    {
        using var scope = _provider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        var auditer = new Auditer(context);
        foreach (var entity in changes)
        {
            await auditer.AuditClaim(entity.Id, entity.Deleted ? "DELETE" : "POST", token);
        }
    }

    private async Task HandleCoverChanges(IReadOnlyCollection<CoverCosmosEntity> changes, CancellationToken token)
    {
        using var scope = _provider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        var auditer = new Auditer(context);
        foreach (var entity in changes)
        {
            await auditer.AuditCover(entity.Id.ToString(), entity.Deleted ? "DELETE" : "POST", token);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var feedProcessor in _processors)
        {
            await feedProcessor.StopAsync();
        }
    }
}