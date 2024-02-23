﻿using Microsoft.Azure.Cosmos;

namespace Claims.Application.Claims;

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

    public async Task<IEnumerable<ClaimCosmosEntity>> GetClaimsAsync(CancellationToken cancellationToken = default)
    {
        var query = _container.GetItemQueryIterator<ClaimCosmosEntity>(new QueryDefinition("SELECT * FROM c"));
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

    public Task AddItemAsync(ClaimCosmosEntity item, CancellationToken cancellationToken = default)
    {
        return _container.CreateItemAsync(item, new PartitionKey(item.Id), cancellationToken: cancellationToken);
    }

    public Task DeleteItemAsync(string id, CancellationToken cancellationToken = default)
    {
        return _container.DeleteItemAsync<ClaimCosmosEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}