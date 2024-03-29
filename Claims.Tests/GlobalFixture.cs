﻿using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MsSql;

namespace Claims.Tests;

public class BaseTest
{
    private MsSqlContainer? _msSqlContainer;
    protected WebApplicationFactory<Program>? Factory { get; private set; }

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
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);
        httpClient.BaseAddress = new Uri("https://localhost:8081");
        var response = await httpClient.GetAsync("/_explorer/index.html");

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Make sure Azure Cosmos DB emulator is working");
    }

    protected static JsonSerializerOptions SerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}