using Claims.Application.Claims;
using Microsoft.Azure.Cosmos;

namespace Claims;

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

    public async Task<IEnumerable<ClaimCosmosEntity>> GetClaimsAsync()
    {
        var query = _container.GetItemQueryIterator<ClaimCosmosEntity>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<ClaimCosmosEntity>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<ClaimCosmosEntity?> GetClaimAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<ClaimCosmosEntity>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task AddItemAsync(ClaimCosmosEntity item)
    {
        return _container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public Task DeleteItemAsync(string id)
    {
        return _container.DeleteItemAsync<ClaimCosmosEntity>(id, new PartitionKey(id));
    }
}