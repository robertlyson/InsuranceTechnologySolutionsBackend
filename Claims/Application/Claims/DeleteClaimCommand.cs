using Claims.Application.Claims.Infrastructure;
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

    public DeleteClaimCommandHandler(ClaimsCosmosRepository claimsCosmosRepository)
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