using Claims.Application.Claims.Dto;
using Claims.Application.Covers;
using Claims.Auditing;
using Claims.Utils;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Claims;

public class CreateClaimCommand : IRequest<Result<ClaimDto>>
{
    public CreateClaimDto Claim { get; }

    public CreateClaimCommand(CreateClaimDto claim)
    {
        Claim = claim;
    }
}

[UsedImplicitly]
public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, Result<ClaimDto>>
{
    private readonly ClaimsCosmosRepository _claimsCosmosRepository;
    private readonly CoversCosmosRepository _coversCosmosRepository;
    private readonly Auditer _auditer;

    public CreateClaimCommandHandler(ClaimsCosmosRepository claimsCosmosRepository, CoversCosmosRepository coversCosmosRepository, Auditer auditer)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
        _coversCosmosRepository = coversCosmosRepository;
        _auditer = auditer;
    }
    
    public async Task<Result<ClaimDto>> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = request.Claim;

        var cover = await _coversCosmosRepository.GetCoverAsync(request.Claim.CoverId?.ToString() ?? string.Empty, cancellationToken);
        if (cover == null)
        {
            return Result<ClaimDto>.Failure(ClaimErrors.CoverNotFound);
        }

        if (claim.DamageCost > 100_000)
        {
            return Result<ClaimDto>.Failure(ClaimErrors.ExceededDamageCost);
        }

        var created = DateOnly.FromDateTime(claim.Created!.Value);
        if (created < cover.StartDate || created > cover.EndDate)
        {
            return Result<ClaimDto>.Failure(ClaimErrors.CreatedDateNotWithinCoverPeriod);
        }
        
        var id = Guid.NewGuid();
        var item = new ClaimCosmosEntity
        {
            Id = id.ToString(),
            Name = claim.Name!,
            Type = claim.ClaimType!.Value,
            CoverId = claim.CoverId!.Value,
            Created = claim.Created!.Value,
            DamageCost = claim.DamageCost!.Value
        };
        await _claimsCosmosRepository.AddItemAsync(item, cancellationToken);
        _auditer.AuditClaim(item.Id, "POST");
        return Result<ClaimDto>.Success(Mappers.ToDto(item));
    }
}