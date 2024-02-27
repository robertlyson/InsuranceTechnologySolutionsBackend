using Domain;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Covers;

public class CoversCosmosRepository : ICoversCosmosRepository
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
        var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.deleted = false OFFSET @offset LIMIT @limit")
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
            return response.Resource.Deleted ? null : response.Resource;
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
            Premium = item.Premium,
            Deleted = false,
        };
        
        return _container.CreateItemAsync(entity, new PartitionKey(entity.Id.ToString()), cancellationToken: cancellationToken);
    }

    public async Task DeleteItemAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await GetCoverAsync(id, cancellationToken);
        if (entity != null)
        {
            entity.Deleted = true;
            await _container.UpsertItemAsync(entity, new PartitionKey(id), new PatchItemRequestOptions { }, cancellationToken);
        }
    }
}