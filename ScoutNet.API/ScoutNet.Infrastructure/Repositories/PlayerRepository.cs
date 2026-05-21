using Microsoft.EntityFrameworkCore;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;
using ScoutNet.Infrastructure.Persistence;
using ScoutNet.Infrastructure.Specifications;

namespace ScoutNet.Infrastructure.Repositories;

public class PlayerRepository(ScoutDbContext dbContext) : IPlayerRepository
{
    public Task<bool> ExistsForLeagueAndSeasonAsync(
        int leagueId,
        int seasonYear,
        CancellationToken cancellationToken = default) =>
        dbContext.Players.AnyAsync(
            player => player.ExternalLeagueId == leagueId &&
                      player.Statistics.Any(statistics => statistics.SeasonYear == seasonYear),
            cancellationToken);

    public Task<bool> ExistsForTeamAndSeasonAsync(
        int teamId,
        int seasonYear,
        CancellationToken cancellationToken = default) =>
        dbContext.Players.AnyAsync(
            player => player.ExternalTeamId == teamId &&
                      player.Statistics.Any(statistics => statistics.SeasonYear == seasonYear),
            cancellationToken);

    public Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Players.FirstOrDefaultAsync(player => player.Id == id, cancellationToken);

    public Task<Player?> GetBySpecAsync(
        ISpecification<Player> spec,
        CancellationToken cancellationToken = default) =>
        SpecificationEvaluator
            .GetQuery(dbContext.Players.AsQueryable(), spec)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Player>> ListBySpecAsync(
        ISpecification<Player> spec,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator
            .GetQuery(dbContext.Players.AsQueryable(), spec)
            .ToListAsync(cancellationToken);

    public Task<int> CountBySpecAsync(
        ISpecification<Player> spec,
        CancellationToken cancellationToken = default) =>
        SpecificationEvaluator
            .GetQuery(dbContext.Players.AsQueryable(), spec)
            .CountAsync(cancellationToken);

    public async Task AddAsync(Player player, CancellationToken cancellationToken = default) =>
        await dbContext.Players.AddAsync(player, cancellationToken);

    public void Update(Player player) => dbContext.Players.Update(player);

    public void Remove(Player player) => dbContext.Players.Remove(player);
}
