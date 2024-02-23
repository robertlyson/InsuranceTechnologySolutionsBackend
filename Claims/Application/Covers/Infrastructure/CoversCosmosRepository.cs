using Microsoft.Azure.Cosmos;

namespace Claims.Application.Covers.Infrastructure;

public class CoversCosmosRepository
{
    private readonly Container _container;
    
    public CoversCosmosRepository(CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<CoverCosmosEntity>> GetCoversAsync(int take, int skip, CancellationToken cancellationToken = default)
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c OFFSET @offset LIMIT @limit")
            .WithParameter("@limit", take)
            .WithParameter("@offset", skip);
        var results = new List<CoverCosmosEntity>();
        var query = _container.GetItemQueryIterator<CoverCosmosEntity>(queryDefinition);
        
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync(cancellationToken);

            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<CoverCosmosEntity?> GetCoverAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<CoverCosmosEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task AddItemAsync(Cover item, CancellationToken cancellationToken = default)
    {
        var entity = new CoverCosmosEntity
        {
            Id = item.Id,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = item.Type,
            Premium = item.Premium
        };
        
        return _container.CreateItemAsync(entity, new PartitionKey(entity.Id.ToString()), cancellationToken: cancellationToken);
    }

    public Task DeleteItemAsync(string id, CancellationToken cancellationToken = default)
    {
        return _container.DeleteItemAsync<CoverCosmosEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}