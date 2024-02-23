using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.Claims;
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
        serviceCollection.AddSingleton(provider =>
            InitializeCosmosClientInstanceAsync(provider.GetRequiredService<CosmosClient>(), configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
    }

    static async Task<ClaimsCosmosRepository> InitializeCosmosClientInstanceAsync(CosmosClient client, IConfigurationSection configurationSection)
    {
        var databaseName = configurationSection.GetSection("DatabaseName").Value;
        var claimsContainerName = configurationSection.GetSection("ClaimContainerName").Value;

        var cosmosDbService = new ClaimsCosmosRepository(client, databaseName, claimsContainerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        var containerResponse = await database.Database.CreateContainerIfNotExistsAsync(claimsContainerName, "/id");

        return cosmosDbService;
    }
}