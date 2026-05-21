namespace ScoutNet.Application.Interfaces.External;

public interface IFootballExternalService
{
    Task FetchAndSavePlayersAsync(
        int teamId,
        int season,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default);
}
