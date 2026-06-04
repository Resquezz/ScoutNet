using Microsoft.EntityFrameworkCore;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;
using ScoutNet.Infrastructure.Persistence;
using ScoutNet.Infrastructure.Specifications;

namespace ScoutNet.Infrastructure.Repositories;

public class WatchlistRepository(ScoutDbContext dbContext) : IWatchlistRepository
{
    public Task<Watchlist?> GetBySpecAsync(
        ISpecification<Watchlist> spec,
        CancellationToken cancellationToken = default) =>
        SpecificationEvaluator
            .GetQuery(dbContext.Watchlists.AsQueryable(), spec)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Watchlist>> ListBySpecAsync(
        ISpecification<Watchlist> spec,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator
            .GetQuery(dbContext.Watchlists.AsQueryable(), spec)
            .ToListAsync(cancellationToken);

    public Task<int> CountBySpecAsync(
        ISpecification<Watchlist> spec,
        CancellationToken cancellationToken = default) =>
        SpecificationEvaluator
            .GetQuery(dbContext.Watchlists.AsQueryable(), spec)
            .CountAsync(cancellationToken);

    public async Task AddAsync(Watchlist watchlist, CancellationToken cancellationToken = default) =>
        await dbContext.Watchlists.AddAsync(watchlist, cancellationToken);

    public void Remove(Watchlist watchlist) => dbContext.Watchlists.Remove(watchlist);
}
