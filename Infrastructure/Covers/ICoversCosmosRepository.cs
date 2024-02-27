using Domain;

namespace Infrastructure.Covers;

public interface ICoversCosmosRepository
{
    Task<IEnumerable<CoverCosmosEntity>> GetCoversAsync(int take, int skip, CancellationToken cancellationToken = default);
    Task<CoverCosmosEntity?> GetCoverAsync(string id, CancellationToken cancellationToken = default);
    Task AddItemAsync(Cover item, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(string id, CancellationToken cancellationToken = default);
}