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
    private readonly CosmosDbService _cosmosDbService;
    private readonly Auditer _auditer;

    public DeleteClaimCommandHandler(CosmosDbService cosmosDbService, Auditer auditer)
    {
        _cosmosDbService = cosmosDbService;
        _auditer = auditer;
    }
    
    public async Task<Unit> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
    {
        var id = request.ClaimId;
        _auditer.AuditClaim(id, "DELETE");
        await _cosmosDbService.DeleteItemAsync(id);

        return new Unit();
    }
}