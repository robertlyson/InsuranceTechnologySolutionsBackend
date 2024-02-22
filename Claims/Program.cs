using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims;
using Claims.Auditing;
using Claims.Controllers;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.AddSingleton(_ =>
{
    var section = builder.Configuration.GetSection("CosmosDb");

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
builder.Services.AddSingleton(provider =>
    InitializeCosmosClientInstanceAsync(provider.GetRequiredService<CosmosClient>(), builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(CosmosClient client, IConfigurationSection configurationSection)
{
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var containerName1 = configurationSection.GetSection("ContainerName1").Value;
    var containerName2 = configurationSection.GetSection("ContainerName2").Value;

    var cosmosDbService = new CosmosDbService(client, databaseName, containerName1);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    var containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerName1, "/id");

    return cosmosDbService;
}

public partial class Program { }