using Claims.Application.Claims.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Claims;

public class GetClaimQuery : IRequest<ClaimDto?>
{
    public string Id { get; }

    public GetClaimQuery(string id)
    {
        Id = id;
    }
}

[UsedImplicitly]
public class GetClaimQueryHandler : IRequestHandler<GetClaimQuery, ClaimDto?>
{
    private readonly ClaimsCosmosRepository _claimsCosmosRepository;

    public GetClaimQueryHandler(ClaimsCosmosRepository claimsCosmosRepository)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
    }
    
    public async Task<ClaimDto?> Handle(GetClaimQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var claim = await _claimsCosmosRepository.GetClaimAsync(id, cancellationToken);
        return claim == null ? null : Mappers.ToDto(claim);
    }
}