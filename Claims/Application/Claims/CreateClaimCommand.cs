using Claims.Application.Claims.Dto;
using Claims.Application.Claims.Infrastructure;
using Claims.Application.Covers;
using Claims.Application.Covers.Infrastructure;
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
        var cover = await _coversCosmosRepository.GetCoverAsync(request.Claim.CoverId?.ToString() ?? string.Empty, cancellationToken);
        if (cover == null)
        {
            return Result<ClaimDto>.Failure(ClaimErrors.CoverNotFound);
        }

        var result = Claim.Create(request.Claim, cover);

        if (result.IsFailure)
        {
            return Result<ClaimDto>.Failure(result.Error);
        }

        var claim = result.Value;
        await _claimsCosmosRepository.AddItemAsync(claim, cancellationToken);
        _auditer.AuditClaim(claim.Id.ToString(), "POST");
        return Result<ClaimDto>.Success(Mappers.ToDto(claim));
    }
}