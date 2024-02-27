using Domain;
using Infrastructure.Covers;
using Infrastructure.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Claims;

public class CreateClaimCommand : IRequest<Result<ClaimDto>>
{
    public CreateClaimCommand(CreateClaimDto claim)
    {
        Claim = claim;
    }

    public CreateClaimDto Claim { get; }
}

[UsedImplicitly]
public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, Result<ClaimDto>>
{
    private readonly IClaimsCosmosRepository _claimsCosmosRepository;
    private readonly ICoversCosmosRepository _coversCosmosRepository;

    public CreateClaimCommandHandler(IClaimsCosmosRepository claimsCosmosRepository,
        ICoversCosmosRepository coversCosmosRepository)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
        _coversCosmosRepository = coversCosmosRepository;
    }

    public async Task<Result<ClaimDto>> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        var cover = await _coversCosmosRepository.GetCoverAsync(request.Claim.CoverId?.ToString() ?? string.Empty,
            cancellationToken);
        if (cover == null) return Result<ClaimDto>.Failure(ClaimErrors.CoverNotFound);

        var result = Claim.Create(new CreateClaim(
                request.Claim.Name ?? string.Empty, request.Claim.Created!.Value, request.Claim.DamageCost!.Value,
                request.Claim.ClaimType!.Value, request.Claim.CoverId!.Value),
            cover.StartDate, cover.EndDate);

        if (result.IsFailure) return Result<ClaimDto>.Failure(result.Error);

        var claim = result.Value;
        await _claimsCosmosRepository.AddItemAsync(claim, cancellationToken);
        return Result<ClaimDto>.Success(Mappers.ToDto(claim));
    }
}