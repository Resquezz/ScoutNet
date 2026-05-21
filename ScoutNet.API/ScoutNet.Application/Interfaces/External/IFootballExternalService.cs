namespace ScoutNet.Application.Interfaces.External;

public interface IFootballExternalService
{
    /// <summary>
    /// Syncs league metadata, teams, and player statistics from API-Football:
    /// leagues → teams?league → players?team (per team).
    /// </summary>
    Task SyncLeagueSeasonAsync(
        int leagueId,
        int season,
        int? teamId = null,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default);
}
