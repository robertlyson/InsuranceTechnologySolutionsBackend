using Infrastructure.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Covers;

public class GetCoversQuery : IRequest<IEnumerable<CoverDto>>
{
    public int Take { get; }
    public int Skip { get; }

    public GetCoversQuery(int take, int skip)
    {
        Take = take;
        Skip = skip;
    }
}

[UsedImplicitly]
public class GetCoversQueryHandler : IRequestHandler<GetCoversQuery, IEnumerable<CoverDto>>
{
    private readonly ICoversCosmosRepository _coversCosmosRepository;

    public GetCoversQueryHandler(ICoversCosmosRepository coversCosmosRepository)
    {
        _coversCosmosRepository = coversCosmosRepository;
    }
    
    public async Task<IEnumerable<CoverDto>> Handle(GetCoversQuery request, CancellationToken cancellationToken)
    {
        var covers = await _coversCosmosRepository.GetCoversAsync(request.Take, request.Skip, cancellationToken);
        return covers.Select(Mappers.ToDto);
    }
}