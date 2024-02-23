using Claims.Auditing;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Claims;

public class DeleteClaimCommand : IRequest<Unit>
{
    public string ClaimId { get; }

    public DeleteClaimCommand(string claimId)
    {
        ClaimId = claimId;
    }
}

[UsedImplicitly]
public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, Unit>
{
    private readonly ClaimsCosmosRepository _claimsCosmosRepository;
    private readonly Auditer _auditer;

    public DeleteClaimCommandHandler(ClaimsCosmosRepository claimsCosmosRepository, Auditer auditer)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
        _auditer = auditer;
    }
    
    public async Task<Unit> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
    {
        var id = request.ClaimId;
        _auditer.AuditClaim(id, "DELETE");
        await _claimsCosmosRepository.DeleteItemAsync(id);

        return new Unit();
    }
}