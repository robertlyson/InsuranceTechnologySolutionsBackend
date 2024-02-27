using Infrastructure.Claims;
using Infrastructure.Covers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class CosmosDbExtensions
{
    public static void AddCosmos(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<CosmosDbOption>()
            .BindConfiguration("CosmosDb")
            .ValidateOnStart();
        serviceCollection.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<CosmosDbOption>>();

            var client = new CosmosClient(options.Value.Account, options.Value.Key);

            return client;
        });
        serviceCollection.AddSingleton<IClaimsCosmosRepository, ClaimsCosmosRepository>(provider =>
            ClaimsCosmosRepositoryAsync(provider.GetRequiredService<CosmosClient>(),
                provider.GetRequiredService<IOptions<CosmosDbOption>>()).GetAwaiter().GetResult());
        serviceCollection.AddSingleton<ICoversCosmosRepository, CoversCosmosRepository>(provider =>
            CoversCosmosRepositoryAsync(provider.GetRequiredService<CosmosClient>(),
                provider.GetRequiredService<IOptions<CosmosDbOption>>()).GetAwaiter().GetResult());
    }

    private static async Task<ClaimsCosmosRepository> ClaimsCosmosRepositoryAsync(CosmosClient client,
        IOptions<CosmosDbOption> options)
    {
        var databaseName = options.Value.DatabaseName!;
        var containerName = options.Value.ClaimsContainerName!;
        var repository = new ClaimsCosmosRepository(client, databaseName, containerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

        return repository;
    }

    private static async Task<CoversCosmosRepository> CoversCosmosRepositoryAsync(CosmosClient client,
        IOptions<CosmosDbOption> options)
    {
        var databaseName = options.Value.DatabaseName!;
        var containerName = options.Value.CoversContainerName!;

        var repository = new CoversCosmosRepository(client, databaseName, containerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

        return repository;
    }
}