using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.Claims;
using Claims.Application.Covers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Claims.Infrastructure;

public static class CosmosDbExtensions
{
    public static void AddCosmos(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<CosmosDbOption>()
            .BindConfiguration("CosmosDb")
            .ValidateDataAnnotations();
        serviceCollection.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<CosmosDbOption>>();
            
            var client = new CosmosClient(options.Value.Account, options.Value.Key);

            return client;
        });
        serviceCollection.AddSingleton<ClaimsCosmosRepository>(provider =>
            ClaimsCosmosRepositoryAsync(provider.GetRequiredService<CosmosClient>(), provider.GetRequiredService<IOptions<CosmosDbOption>>()).GetAwaiter().GetResult());
        serviceCollection.AddSingleton<CoversCosmosRepository>(provider =>
            CoversCosmosRepositoryAsync(provider.GetRequiredService<CosmosClient>(), provider.GetRequiredService<IOptions<CosmosDbOption>>()).GetAwaiter().GetResult());
    }

    static async Task<ClaimsCosmosRepository> ClaimsCosmosRepositoryAsync(CosmosClient client, IOptions<CosmosDbOption> options)
    {
        var databaseName = options.Value.DatabaseName!;
        var containerName = options.Value.ClaimsContainerName!;
        var repository = new ClaimsCosmosRepository(client, databaseName, containerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

        return repository;
    }

    static async Task<CoversCosmosRepository> CoversCosmosRepositoryAsync(CosmosClient client, IOptions<CosmosDbOption> options)
    {
        var databaseName = options.Value.DatabaseName!;
        var containerName = options.Value.ClaimsContainerName!;

        var repository = new CoversCosmosRepository(client, databaseName, containerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

        return repository;
    }
}