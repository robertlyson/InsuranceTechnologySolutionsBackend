using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.Claims;
using Claims.Application.Covers;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure;

public static class CosmosDbExtensions
{
    public static void AddCosmos(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton(_ =>
        {
            var section = configuration.GetSection("CosmosDb");

            var account = section.GetSection("Account").Value;
            var key = section.GetSection("Key").Value;

            var client = new CosmosClient(account, key);

            return client;
        });
        serviceCollection.AddSingleton<ClaimsCosmosRepository>(provider =>
            ClaimsCosmosRepositoryAsync(provider.GetRequiredService<CosmosClient>(), configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        serviceCollection.AddSingleton<CoversCosmosRepository>(provider =>
            CoversCosmosRepositoryAsync(provider.GetRequiredService<CosmosClient>(), configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
    }

    static async Task<ClaimsCosmosRepository> ClaimsCosmosRepositoryAsync(CosmosClient client, IConfigurationSection configurationSection)
    {
        var databaseName = configurationSection.GetSection("DatabaseName").Value;
        var claimsContainerName = configurationSection.GetSection("ClaimsContainerName").Value;

        var repository = new ClaimsCosmosRepository(client, databaseName, claimsContainerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(claimsContainerName, "/id");

        return repository;
    }

    static async Task<CoversCosmosRepository> CoversCosmosRepositoryAsync(CosmosClient client, IConfigurationSection configurationSection)
    {
        var databaseName = configurationSection.GetSection("DatabaseName").Value;
        var coversContainerName = configurationSection.GetSection("CoversContainerName").Value;

        var repository = new CoversCosmosRepository(client, databaseName, coversContainerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(coversContainerName, "/id");

        return repository;
    }
}