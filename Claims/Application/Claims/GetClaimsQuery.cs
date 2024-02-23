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
    private readonly ClaimsCosmosRepository _claimsCosmosRepository;

    public GetClaimsQueryHandler(ClaimsCosmosRepository claimsCosmosRepository)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
    }
    
    public async Task<ClaimDto[]> Handle(GetClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await _claimsCosmosRepository.GetClaimsAsync();
        return claims.Select(Mappers.ToDto).ToArray();
    }
}