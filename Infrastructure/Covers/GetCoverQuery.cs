using Infrastructure.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Covers;

public class GetCoverQuery : IRequest<CoverDto?>
{
    public string Id { get; }

    public GetCoverQuery(string id)
    {
        Id = id;
    }
}

[UsedImplicitly]
public class GetCoverQueryHandler : IRequestHandler<GetCoverQuery, CoverDto?>
{
    private readonly ICoversCosmosRepository _repository;

    public GetCoverQueryHandler(ICoversCosmosRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CoverDto?> Handle(GetCoverQuery request, CancellationToken cancellationToken)
    {
        var cover = await _repository.GetCoverAsync(request.Id, cancellationToken);
        return cover == null ? null : Mappers.ToDto(cover);
    }
}