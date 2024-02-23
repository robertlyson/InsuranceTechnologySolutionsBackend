using Claims.Application.Covers.Dto;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Claims.Application.Covers;

public class GetCoversQuery : IRequest<IEnumerable<CoverDto>>
{
    
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
        var covers = await _coversCosmosRepository.GetCoversAsync(cancellationToken);
        return covers.Select(Mappers.ToDto);
    }
}