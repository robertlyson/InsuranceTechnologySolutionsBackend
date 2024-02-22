using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Claims.Tests;

[SetUpFixture]
public class ApplicationFixture
{
    protected WebApplicationFactory<Program>? Factory { get; private set; }

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("CosmosDb:ConnectionString", "");
            });
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await Factory!.DisposeAsync();
    }
}