using Claims.Application.Claims.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Claims;

public class GetClaimsQuery : IRequest<ClaimDto[]>
{
    
}

[UsedImplicitly]
public class GetClaimsQueryHandler : IRequestHandler<GetClaimsQuery, ClaimDto[]>
{
    private readonly CosmosDbService _cosmosDbService;

    public GetClaimsQueryHandler(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }
    
    public async Task<ClaimDto[]> Handle(GetClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await _cosmosDbService.GetClaimsAsync();
        return claims.Select(Mappers.ToDto).ToArray();
    }
}