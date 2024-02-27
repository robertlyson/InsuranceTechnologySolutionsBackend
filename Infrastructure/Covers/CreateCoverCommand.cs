using Domain;
using Infrastructure.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Covers;

public class CreateCoverCommand : IRequest<Result<CoverDto>>
{
    public CreateCoverCommand(CreateCoverDto cover)
    {
        Cover = cover;
    }

    public CreateCoverDto Cover { get; }
}

[UsedImplicitly]
public class CreateCoverCommandHandler : IRequestHandler<CreateCoverCommand, Result<CoverDto>>
{
    private readonly IPremiumStrategy _premiumStrategy;
    private readonly ICoversCosmosRepository _repository;

    public CreateCoverCommandHandler(ICoversCosmosRepository repository, IPremiumStrategy premiumStrategy)
    {
        _repository = repository;
        _premiumStrategy = premiumStrategy;
    }

    public async Task<Result<CoverDto>> Handle(CreateCoverCommand request, CancellationToken cancellationToken)
    {
        var result =
            Cover.Create(
                new CreateCover(request.Cover.StartDate!.Value, request.Cover.EndDate!.Value,
                    request.Cover.CoverType!.Value), _premiumStrategy);

        await _repository.AddItemAsync(result.Value, cancellationToken);

        return Result<CoverDto>.Success(Mappers.ToDto(result.Value));
    }
}