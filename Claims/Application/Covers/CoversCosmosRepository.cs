using Claims.Application.Claims;
using Microsoft.Azure.Cosmos;

namespace Claims.Application.Covers;

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

    public async Task<IEnumerable<CoverCosmosEntity>> GetCoversAsync(CancellationToken cancellationToken = default)
    {
        var query = _container.GetItemQueryIterator<CoverCosmosEntity>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<CoverCosmosEntity>();
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

    public Task AddItemAsync(CoverCosmosEntity item, CancellationToken cancellationToken = default)
    {
        return _container.CreateItemAsync(item, new PartitionKey(item.Id), cancellationToken: cancellationToken);
    }

    public Task DeleteItemAsync(string id, CancellationToken cancellationToken = default)
    {
        return _container.DeleteItemAsync<CoverCosmosEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}