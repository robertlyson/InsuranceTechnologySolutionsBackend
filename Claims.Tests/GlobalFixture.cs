using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace Claims.Tests;

public class BaseTest
{
    protected WebApplicationFactory<Program>? Factory { get; private set; }
    private MsSqlContainer? _msSqlContainer;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _msSqlContainer = new MsSqlBuilder()
            .Build();

        await _msSqlContainer.StartAsync();
        var connectionString = _msSqlContainer.GetConnectionString();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);
            });
        await ValidateCosmosDb();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await Factory!.DisposeAsync();
        await _msSqlContainer!.DisposeAsync();
    }

    private async Task ValidateCosmosDb()
    {
        //validate if azure cosmos db is running
        using var httpClient = Factory!.CreateClient();
        var response = await httpClient.GetAsync("https://localhost:8081/_explorer/index.html");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Make sure Azure Cosmos DB emulator is working");
        }
    }
}