using Claims.Application.Covers.Dto;
using Claims.Application.Covers.Infrastructure;
using Claims.Auditing;
using Claims.Utils;
using JetBrains.Annotations;
using MediatR;

namespace Claims.Application.Covers;

public class CreateCoverCommand : IRequest<Result<CoverDto>>
{
    public CreateCoverDto Cover { get; }

    public CreateCoverCommand(CreateCoverDto cover)
    {
        Cover = cover;
    }
}

[UsedImplicitly]
public class CreateCoverCommandHandler : IRequestHandler<CreateCoverCommand, Result<CoverDto>>
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
    
    public async Task<Result<CoverDto>> Handle(CreateCoverCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var result = Cover.Create(request.Cover, _premiumStrategy);

        await _repository.AddItemAsync(result.Value, cancellationToken);
        
        _auditer.AuditCover(id.ToString(), "POST");

        return Result<CoverDto>.Success(Mappers.ToDto(result.Value));
    }
}