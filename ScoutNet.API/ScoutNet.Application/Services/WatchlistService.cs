using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.Application.Mapping;
using ScoutNet.Application.Specifications;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Services;

public class WatchlistService(
    IWatchlistRepository watchlistRepository,
    IPlayerRepository playerRepository,
    IUnitOfWork unitOfWork) : IWatchlistService
{
    public async Task<IReadOnlyList<WatchlistItemDto>> GetMyWatchlistAsync(
        Guid scoutId,
        CancellationToken cancellationToken = default)
    {
        var entries = await watchlistRepository.ListBySpecAsync(
            new WatchlistByScoutSpecification(scoutId),
            cancellationToken);

        return entries
            .Select(entry => new WatchlistItemDto
            {
                PlayerId = entry.Player.ExternalId,
                Player = PlayerMapper.ToDto(entry.Player),
            })
            .ToList();
    }

    public async Task AddPlayerAsync(
        Guid scoutId,
        int playerExternalId,
        CancellationToken cancellationToken = default)
    {
        var player = await playerRepository.GetBySpecAsync(
            new PlayerByExternalIdWithStatisticsSpecification(playerExternalId),
            cancellationToken);

        if (player is null)
        {
            throw new KeyNotFoundException($"Player with id '{playerExternalId}' was not found.");
        }

        var existing = await watchlistRepository.GetBySpecAsync(
            new WatchlistEntrySpecification(scoutId, player.Id),
            cancellationToken);

        if (existing is not null)
        {
            return;
        }

        await watchlistRepository.AddAsync(new Watchlist
        {
            ScoutId = scoutId,
            PlayerId = player.Id,
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePlayerAsync(
        Guid scoutId,
        int playerExternalId,
        CancellationToken cancellationToken = default)
    {
        var player = await playerRepository.GetBySpecAsync(
            new PlayerByExternalIdWithStatisticsSpecification(playerExternalId),
            cancellationToken);

        if (player is null)
        {
            throw new KeyNotFoundException($"Player with id '{playerExternalId}' was not found.");
        }

        var entry = await watchlistRepository.GetBySpecAsync(
            new WatchlistEntrySpecification(scoutId, player.Id),
            cancellationToken);

        if (entry is null)
        {
            throw new KeyNotFoundException("Player is not in your watchlist.");
        }

        watchlistRepository.Remove(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
