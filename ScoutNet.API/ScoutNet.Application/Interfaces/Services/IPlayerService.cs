using ScoutNet.Application.DTOs;

namespace ScoutNet.Application.Interfaces.Services;

public interface IPlayerService
{
    Task<IReadOnlyList<PlayerDto>> GetPlayersWithSyncAsync(
        PlayerFilterDto filter,
        int season,
        int leagueId,
        CancellationToken cancellationToken = default);

    Task<PlayerComparisonDto> ComparePlayersAsync(
        int id1,
        int id2,
        int season,
        CancellationToken cancellationToken = default);
}
