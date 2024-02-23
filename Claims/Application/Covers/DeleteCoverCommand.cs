using Claims.Application.Covers.Infrastructure;
using Claims.Auditing;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Covers;

public class DeleteCoverCommand : IRequest<Unit>
{
    public string Id { get; }

    public DeleteCoverCommand(string id)
    {
        Id = id;
    }
}

[UsedImplicitly]
public class DeleteCoverCommandHandler : IRequestHandler<DeleteCoverCommand, Unit>
{
    private readonly CoversCosmosRepository _repository;
    private readonly Auditer _auditer;

    public DeleteCoverCommandHandler(CoversCosmosRepository repository, Auditer auditer)
    {
        _repository = repository;
        _auditer = auditer;
    }
    
    public async Task<Unit> Handle(DeleteCoverCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        _auditer.AuditCover(id, "DELETE");
        await _repository.DeleteItemAsync(id, cancellationToken);

        return new Unit();
    }
}