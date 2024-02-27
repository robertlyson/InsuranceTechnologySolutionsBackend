using Domain;

namespace Infrastructure.Claims;

public interface IClaimsCosmosRepository
{
    Task<IEnumerable<ClaimCosmosEntity>> GetClaimsAsync(int take, int skip, string? name = null,
        CancellationToken cancellationToken = default);

    Task<ClaimCosmosEntity?> GetClaimAsync(string id, CancellationToken cancellationToken = default);
    Task AddItemAsync(Claim item, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(string id, CancellationToken cancellationToken = default);
}