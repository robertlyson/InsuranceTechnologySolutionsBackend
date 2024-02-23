using Claims.Controllers.Covers.Dto;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Claims.Application.Covers;

public class GetCoversQuery : IRequest<IEnumerable<CoverDto>>
{
    
}

[UsedImplicitly]
public class GetCoversQueryHandler : IRequestHandler<GetCoversQuery, IEnumerable<CoverDto>>
{
    private readonly CosmosClient _cosmosClient;

    public GetCoversQueryHandler(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }
    
    public async Task<IEnumerable<CoverDto>> Handle(GetCoversQuery request, CancellationToken cancellationToken)
    {
        var container = await Container();
        var query = container.GetItemQueryIterator<CoverCosmosEntity>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<CoverDto>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.Select(Mappers.ToDto).ToList());
        }

        return results;
    }

    private async Task<Container> Container()
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync("ClaimDb");
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync("Cover", "/id");
        var container = containerResponse.Container;
        return container;
    }
}