using Claims.Application.Claims.Dto;
using Claims.Auditing;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Claims;

public class CreateClaimCommand : IRequest<ClaimDto>
{
    public CreateClaimDto Claim { get; }

    public CreateClaimCommand(CreateClaimDto claim)
    {
        Claim = claim;
    }
}

[UsedImplicitly]
public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, ClaimDto>
{
    private readonly ClaimsCosmosRepository _claimsCosmosRepository;
    private readonly Auditer _auditer;

    public CreateClaimCommandHandler(ClaimsCosmosRepository claimsCosmosRepository, Auditer auditer)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
        _auditer = auditer;
    }
    
    public async Task<ClaimDto> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = request.Claim;
        var id = Guid.NewGuid();
        var item = new ClaimCosmosEntity
        {
            Id = id.ToString(),
            Name = claim.Name!,
            Type = claim.ClaimType!.Value,
            CoverId = claim.CoverId!.Value.ToString(),
            Created = claim.Created!.Value,
            DamageCost = claim.DamageCost!.Value
        };
        await _claimsCosmosRepository.AddItemAsync(item);
        _auditer.AuditClaim(item.Id, "POST");
        return Mappers.ToDto(item);
    }
}