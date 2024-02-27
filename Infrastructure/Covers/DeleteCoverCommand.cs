using Infrastructure.Covers;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Covers;

public class DeleteCoverCommand : IRequest<Unit>
{
    public DeleteCoverCommand(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

[UsedImplicitly]
public class DeleteCoverCommandHandler : IRequestHandler<DeleteCoverCommand, Unit>
{
    private readonly ICoversCosmosRepository _repository;

    public DeleteCoverCommandHandler(ICoversCosmosRepository repository)
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