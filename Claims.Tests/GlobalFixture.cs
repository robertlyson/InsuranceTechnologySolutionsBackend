using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Testcontainers.CosmosDb;

namespace Claims.Tests;

[SetUpFixture]
public class ApplicationFixture
{
    private CosmosDbContainer? _cosmosDbContainer;
    protected WebApplicationFactory<Program>? Factory { get; private set; }

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _cosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1")
            .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
            .WithCleanUp(true)
            .Build();
        await _cosmosDbContainer.StartAsync();

        var cosmosDbConnectionString = _cosmosDbContainer.GetConnectionString();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("CosmosDb:ConnectionString", cosmosDbConnectionString);
            });
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _cosmosDbContainer!.DisposeAsync();
        await Factory!.DisposeAsync();
    }
}