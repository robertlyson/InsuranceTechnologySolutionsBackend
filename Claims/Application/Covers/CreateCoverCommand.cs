using Claims.Application.Covers.Dto;
using Claims.Auditing;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Covers;

public class CreateCoverCommand : IRequest<CoverDto>
{
    public CreateCoverDto Cover { get; }

    public CreateCoverCommand(CreateCoverDto cover)
    {
        Cover = cover;
    }
}

[UsedImplicitly]
public class CreateCoverCommandHandler : IRequestHandler<CreateCoverCommand, CoverDto>
{
    private readonly CoversCosmosRepository _repository;
    private readonly Auditer _auditer;
    private readonly IPremiumStrategy _premiumStrategy;

    public CreateCoverCommandHandler(CoversCosmosRepository repository, Auditer auditer, IPremiumStrategy premiumStrategy)
    {
        _repository = repository;
        _auditer = auditer;
        _premiumStrategy = premiumStrategy;
    }
    
    public async Task<CoverDto> Handle(CreateCoverCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var cover = request.Cover;
        var premium = _premiumStrategy.Calculate(cover.StartDate!.Value, cover.EndDate!.Value, cover.CoverType!.Value);
        var item = new CoverCosmosEntity
        {
            Id = id,
            Type = cover.CoverType!.Value,
            StartDate = cover.StartDate!.Value,
            EndDate = cover.EndDate!.Value,
            Premium = premium
        };

        await _repository.AddItemAsync(item, cancellationToken);
        
        _auditer.AuditCover(id.ToString(), "POST");

        return Mappers.ToDto(item);
    }
}