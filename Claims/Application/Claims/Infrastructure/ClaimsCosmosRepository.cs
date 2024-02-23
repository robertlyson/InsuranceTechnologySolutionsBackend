using Microsoft.Azure.Cosmos;

namespace Claims.Application.Claims.Infrastructure;

public class ClaimsCosmosRepository
{
    private readonly Container _container;

    public ClaimsCosmosRepository(CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<ClaimCosmosEntity>> GetClaimsAsync(int take, int skip, string? name = null, CancellationToken cancellationToken = default)
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c OFFSET @offset LIMIT @limit")
            .WithParameter("@limit", take)
            .WithParameter("@offset", skip);

        if (!string.IsNullOrEmpty(name))
        {
            queryDefinition = new QueryDefinition("SELECT * FROM c WHERE STARTSWITH(c.name, @name) OFFSET @offset LIMIT @limit")
                .WithParameter("@name", name)
                .WithParameter("@limit", take)
                .WithParameter("@offset", skip);
        }
        
        var query = _container.GetItemQueryIterator<ClaimCosmosEntity>(queryDefinition);
        var results = new List<ClaimCosmosEntity>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync(cancellationToken);

            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<ClaimCosmosEntity?> GetClaimAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<ClaimCosmosEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task AddItemAsync(Claim item, CancellationToken cancellationToken = default)
    {
        var entity = new ClaimCosmosEntity
        {
            Id = item.Id.ToString(),
            Name = item.Name,
            DamageCost = item.DamageCost,
            Created = item.Created,
            CoverId = item.CoverId,
            Type = item.Type
        };

        return _container.CreateItemAsync(entity, new PartitionKey(entity.Id), cancellationToken: cancellationToken);
    }

    public Task DeleteItemAsync(string id, CancellationToken cancellationToken = default)
    {
        return _container.DeleteItemAsync<ClaimCosmosEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}