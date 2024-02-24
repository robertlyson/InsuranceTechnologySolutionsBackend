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

    public DeleteCoverCommandHandler(CoversCosmosRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Unit> Handle(DeleteCoverCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        await _repository.DeleteItemAsync(id, cancellationToken);

        return new Unit();
    }
}