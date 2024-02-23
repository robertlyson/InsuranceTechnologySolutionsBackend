using Claims.Application.Covers.Dto;
using Claims.Application.Covers.Infrastructure;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Claims.Application.Covers;

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
    private readonly CoversCosmosRepository _coversCosmosRepository;

    public GetCoversQueryHandler(CoversCosmosRepository coversCosmosRepository)
    {
        _coversCosmosRepository = coversCosmosRepository;
    }
    
    public async Task<IEnumerable<CoverDto>> Handle(GetCoversQuery request, CancellationToken cancellationToken)
    {
        var covers = await _coversCosmosRepository.GetCoversAsync(request.Take, request.Skip, cancellationToken);
        return covers.Select(Mappers.ToDto);
    }
}