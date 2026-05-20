using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Interfaces.Repositories;

public interface IWatchlistRepository
{
    Task<Watchlist?> GetBySpecAsync(ISpecification<Watchlist> spec, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Watchlist>> ListBySpecAsync(ISpecification<Watchlist> spec, CancellationToken cancellationToken = default);

    Task<int> CountBySpecAsync(ISpecification<Watchlist> spec, CancellationToken cancellationToken = default);

    Task AddAsync(Watchlist watchlist, CancellationToken cancellationToken = default);

    void Remove(Watchlist watchlist);
}
