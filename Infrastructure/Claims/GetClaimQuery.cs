using Infrastructure.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Claims;

public class GetClaimQuery : IRequest<ClaimDto?>
{
    public GetClaimQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

[UsedImplicitly]
public class GetClaimQueryHandler : IRequestHandler<GetClaimQuery, ClaimDto?>
{
    private readonly IClaimsCosmosRepository _claimsCosmosRepository;

    public GetClaimQueryHandler(IClaimsCosmosRepository claimsCosmosRepository)
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