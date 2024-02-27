using Infrastructure.Dto;
using JetBrains.Annotations;
using MediatR;

namespace Infrastructure.Claims;

public class GetClaimsQuery : IRequest<ClaimDto[]>
{
    public int Take { get; }
    public int Skip { get; }
    public string? Name { get; }

    public GetClaimsQuery(int take, int skip, string? name)
    {
        Take = take;
        Skip = skip;
        Name = name;
    }
}

[UsedImplicitly]
public class GetClaimsQueryHandler : IRequestHandler<GetClaimsQuery, ClaimDto[]>
{
    private readonly IClaimsCosmosRepository _claimsCosmosRepository;

    public GetClaimsQueryHandler(IClaimsCosmosRepository claimsCosmosRepository)
    {
        _claimsCosmosRepository = claimsCosmosRepository;
    }
    
    public async Task<ClaimDto[]> Handle(GetClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await _claimsCosmosRepository.GetClaimsAsync(request.Take, request.Skip, request.Name, cancellationToken);
        return claims.Select(Mappers.ToDto).ToArray();
    }
}