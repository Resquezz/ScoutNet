namespace ScoutNet.Application.Interfaces.External;

public interface IFootballExternalService
{
    Task FetchAndSavePlayersAsync(int leagueId, int season, CancellationToken cancellationToken = default);
}
