using Infrastructure.Claims;
using Infrastructure.Covers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Auditing;

public class AuditHostedService : IHostedService
{
    private readonly CosmosClient _cosmosClient;
    private readonly IOptions<CosmosDbOption> _options;
    private readonly List<ChangeFeedProcessor> _processors = new();
    private readonly IServiceProvider _provider;

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
        var databaseResponse =
            await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName, cancellationToken: cancellationToken);
        var database = databaseResponse.Database;

        var claimsProcessor = await BuildProcessor<ClaimCosmosEntity>(database, claimsContainerName,
            HandleClaimChanges, cancellationToken);
        _processors.Add(claimsProcessor);
        var coversProcessor = await BuildProcessor<CoverCosmosEntity>(database, coversContainerName,
            HandleCoverChanges, cancellationToken);
        _processors.Add(coversProcessor);

        foreach (var feedProcessor in _processors) await feedProcessor.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var feedProcessor in _processors) await feedProcessor.StopAsync();
    }

    private async Task<ChangeFeedProcessor> BuildProcessor<T>(Database database, string? containerName,
        Container.ChangesHandler<T> changesHandler, CancellationToken cancellationToken)
    {
        var sourceContainer =
            await database.CreateContainerIfNotExistsAsync(containerName, "/id", cancellationToken: cancellationToken);
        var leaseContainer = await database.CreateContainerIfNotExistsAsync($"{containerName}_lease", "/id",
            cancellationToken: cancellationToken);

        var builder = sourceContainer.Container.GetChangeFeedProcessorBuilder(
            $"{containerName}-processor",
            changesHandler
        );

        return builder
            .WithInstanceName("claimsApp")
            .WithLeaseContainer(leaseContainer)
            .Build();
    }

    private async Task HandleClaimChanges(IReadOnlyCollection<ClaimCosmosEntity> changes, CancellationToken token)
    {
        using var scope = _provider.CreateScope();
        var auditer = scope.ServiceProvider.GetRequiredService<IAuditer>();
        foreach (var entity in changes) await auditer.AuditClaim(entity.Id, entity.Deleted ? "DELETE" : "POST", token);
    }

    private async Task HandleCoverChanges(IReadOnlyCollection<CoverCosmosEntity> changes, CancellationToken token)
    {
        using var scope = _provider.CreateScope();
        var auditer = scope.ServiceProvider.GetRequiredService<IAuditer>();
        foreach (var entity in changes)
            await auditer.AuditCover(entity.Id.ToString(), entity.Deleted ? "DELETE" : "POST", token);
    }
}