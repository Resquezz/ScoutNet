using ScoutNet.Application.DTOs;

namespace ScoutNet.Application.Interfaces.Services;

public interface IWatchlistService
{
    Task<IReadOnlyList<WatchlistItemDto>> GetMyWatchlistAsync(
        Guid scoutId,
        CancellationToken cancellationToken = default);

    Task AddPlayerAsync(
        Guid scoutId,
        int playerExternalId,
        CancellationToken cancellationToken = default);

    Task RemovePlayerAsync(
        Guid scoutId,
        int playerExternalId,
        CancellationToken cancellationToken = default);
}
