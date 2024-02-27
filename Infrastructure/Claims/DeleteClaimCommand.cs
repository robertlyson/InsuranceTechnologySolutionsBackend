using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Claims;

public class DeleteClaimCommand : IRequest<Unit>
{
    public DeleteClaimCommand(string claimId)
    {
        ClaimId = claimId;
    }

    public string ClaimId { get; }
}

[UsedImplicitly]
public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, Unit>
{
    private readonly IClaimsCosmosRepository _claimsCosmosRepository;

    public DeleteClaimCommandHandler(IClaimsCosmosRepository claimsCosmosRepository)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
    }

    public async Task<Unit> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
    {
        var id = request.ClaimId;
        await _claimsCosmosRepository.DeleteItemAsync(id, cancellationToken);

        return new Unit();
    }
}