using System.Text.Json;
using System.Text.Json.Serialization;
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
    
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var client = new CosmosClient(account, key,
                new CosmosClientOptions { Serializer = new CosmosSystemTextJsonSerializer(options) });

            return client;
        });
        serviceCollection.AddSingleton(provider =>
            InitializeCosmosClientInstanceAsync(provider.GetRequiredService<CosmosClient>(), configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
    }

    static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(CosmosClient client, IConfigurationSection configurationSection)
    {
        var databaseName = configurationSection.GetSection("DatabaseName").Value;
        var containerName1 = configurationSection.GetSection("ContainerName").Value;

        var cosmosDbService = new CosmosDbService(client, databaseName, containerName1);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        var containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerName1, "/id");

        return cosmosDbService;
    }
}