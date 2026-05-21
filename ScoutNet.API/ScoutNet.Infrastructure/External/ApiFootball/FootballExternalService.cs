using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoutNet.Application.Interfaces.External;
using ScoutNet.Domain.Entities;
using ScoutNet.Infrastructure.External.ApiFootball.Models;
using ScoutNet.Infrastructure.Options;
using ScoutNet.Infrastructure.Persistence;

namespace ScoutNet.Infrastructure.External.ApiFootball;

public class FootballExternalService(
    HttpClient httpClient,
    ScoutDbContext dbContext,
    IOptions<ApiFootballOptions> options,
    ILogger<FootballExternalService> logger) : IFootballExternalService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task SyncLeagueSeasonAsync(
        int leagueId,
        int season,
        int? teamId = null,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (forceRefresh)
        {
            await RemoveLeagueSeasonDataAsync(leagueId, season, teamId, cancellationToken);
        }

        var teamCache = await LoadTeamCacheAsync(cancellationToken);
        var leagueCache = await LoadLeagueCacheAsync(cancellationToken);
        var newTeams = new List<Team>();
        var newLeagues = new List<League>();

        await EnsureLeagueFromApiAsync(
            leagueId,
            season,
            leagueCache,
            newLeagues,
            cancellationToken);

        var leagueTeamIds = await EnsureTeamsFromApiAsync(
            leagueId,
            season,
            teamCache,
            newTeams,
            cancellationToken);

        if (newLeagues.Count > 0)
        {
            await dbContext.Leagues.AddRangeAsync(newLeagues, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            newLeagues.Clear();
        }

        if (newTeams.Count > 0)
        {
            await dbContext.Teams.AddRangeAsync(newTeams, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            newTeams.Clear();
        }

        var teamsToImport = teamId.HasValue
            ? [teamId.Value]
            : leagueTeamIds;

        if (teamId.HasValue && !leagueTeamIds.Contains(teamId.Value))
        {
            throw new InvalidOperationException(
                $"Team {teamId.Value} is not part of league {leagueId} for season {season}.");
        }

        foreach (var externalTeamId in teamsToImport)
        {
            await ImportPlayersForTeamAsync(
                externalTeamId,
                season,
                leagueId,
                teamCache,
                leagueCache,
                newTeams,
                newLeagues,
                cancellationToken);
        }

        if (newTeams.Count > 0)
        {
            await dbContext.Teams.AddRangeAsync(newTeams, cancellationToken);
        }

        if (newLeagues.Count > 0)
        {
            await dbContext.Leagues.AddRangeAsync(newLeagues, cancellationToken);
        }

        if (newTeams.Count > 0 || newLeagues.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task EnsureLeagueFromApiAsync(
        int leagueId,
        int season,
        Dictionary<int, League> leagueCache,
        List<League> newLeagues,
        CancellationToken cancellationToken)
    {
        if (leagueCache.ContainsKey(leagueId))
        {
            return;
        }

        var apiOptions = options.Value;
        var path = $"{apiOptions.LeaguesEndpoint}?id={leagueId}&season={season}";
        var items = await FetchAsync<ApiFootballLeagueListItemDto>(path, cancellationToken);
        var leagueItem = items.FirstOrDefault();

        if (leagueItem is null)
        {
            throw new InvalidOperationException(
                $"API-Football returned no league data for league {leagueId}, season {season}.");
        }

        var league = ApiFootballImportMapper.MapLeague(leagueItem.League, leagueItem.Country);
        leagueCache[leagueId] = league;
        newLeagues.Add(league);

        logger.LogInformation(
            "Loaded league {LeagueName} ({LeagueId}) for season {Season} from API-Football.",
            league.Name,
            leagueId,
            season);
    }

    private async Task<IReadOnlyList<int>> EnsureTeamsFromApiAsync(
        int leagueId,
        int season,
        Dictionary<int, Team> teamCache,
        List<Team> newTeams,
        CancellationToken cancellationToken)
    {
        var apiOptions = options.Value;
        var path = $"{apiOptions.TeamsEndpoint}?league={leagueId}&season={season}";
        var items = await FetchAllPagesAsync<ApiFootballTeamListItemDto>(path, cancellationToken);

        if (items.Count == 0)
        {
            throw new InvalidOperationException(
                $"API-Football returned no teams for league {leagueId}, season {season}.");
        }

        foreach (var item in items)
        {
            ResolveTeam(item.Team, teamCache, newTeams);
        }

        logger.LogInformation(
            "Loaded {TeamCount} teams for league {LeagueId}, season {Season} from API-Football.",
            items.Count,
            leagueId,
            season);

        return items.Select(item => item.Team.Id).Distinct().ToList();
    }

    private async Task ImportPlayersForTeamAsync(
        int teamId,
        int season,
        int leagueId,
        Dictionary<int, Team> teamCache,
        Dictionary<int, League> leagueCache,
        List<Team> newTeams,
        List<League> newLeagues,
        CancellationToken cancellationToken)
    {
        var apiItems = await FetchAllPlayersFromApiAsync(teamId, season, cancellationToken);
        var externalIds = apiItems.Select(item => item.Player.Id).Distinct().ToList();

        var existingPlayers = await dbContext.Players
            .Where(player => externalIds.Contains(player.ExternalId))
            .Include(player => player.Statistics)
            .ToDictionaryAsync(player => player.ExternalId, cancellationToken);

        var newPlayers = new List<Player>();
        var processedExternalIds = new HashSet<int>();
        var importedCount = 0;
        var updatedCount = 0;
        var statisticsCount = 0;

        foreach (var item in apiItems)
        {
            if (!processedExternalIds.Add(item.Player.Id))
            {
                continue;
            }

            var teamStatistics = item.Statistics
                .Where(statistics =>
                    statistics.Team.Id == teamId &&
                    statistics.League.Id == leagueId &&
                    statistics.League.Season == season)
                .ToList();

            if (teamStatistics.Count == 0)
            {
                continue;
            }

            foreach (var statisticsDto in teamStatistics)
            {
                ResolveTeam(statisticsDto.Team, teamCache, newTeams);
                ResolveLeague(statisticsDto.League, leagueCache, newLeagues);
            }

            var primaryStatistics = teamStatistics
                .OrderByDescending(statistics => statistics.Games.Appearences ?? statistics.Games.Lineups ?? 0)
                .First();

            var team = teamCache[primaryStatistics.Team.Id];
            var league = leagueCache[primaryStatistics.League.Id];

            if (existingPlayers.TryGetValue(item.Player.Id, out var existingPlayer))
            {
                ApiFootballImportMapper.UpdatePlayer(
                    existingPlayer,
                    item,
                    primaryStatistics,
                    team,
                    league,
                    teamId);

                statisticsCount += UpsertPlayerStatistics(
                    existingPlayer,
                    teamStatistics,
                    teamCache,
                    leagueCache);

                updatedCount++;
                continue;
            }

            var player = ApiFootballImportMapper.MapPlayer(item, primaryStatistics, team, league, teamId);
            player.Statistics = teamStatistics
                .Select(statisticsDto =>
                    ApiFootballImportMapper.MapStatistics(
                        statisticsDto,
                        player.Id,
                        teamCache[statisticsDto.Team.Id],
                        leagueCache[statisticsDto.League.Id]))
                .ToList();

            existingPlayers[item.Player.Id] = player;
            newPlayers.Add(player);
            statisticsCount += player.Statistics.Count;
            importedCount++;
        }

        if (importedCount == 0 && updatedCount == 0)
        {
            logger.LogWarning(
                "API-Football returned no mappable players for team {TeamId}, league {LeagueId}, season {Season}.",
                teamId,
                leagueId,
                season);
            return;
        }

        if (newPlayers.Count > 0)
        {
            await dbContext.Players.AddRangeAsync(newPlayers, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Synced team {TeamId}, league {LeagueId}, season {Season}: {ImportedCount} new players, {UpdatedCount} updated, {StatisticsCount} statistics rows.",
            teamId,
            leagueId,
            season,
            importedCount,
            updatedCount,
            statisticsCount);
    }

    private static int UpsertPlayerStatistics(
        Player player,
        IReadOnlyList<ApiFootballPlayerStatisticsDto> teamStatistics,
        Dictionary<int, Team> teamCache,
        Dictionary<int, League> leagueCache)
    {
        var count = 0;

        foreach (var statisticsDto in teamStatistics)
        {
            var team = teamCache[statisticsDto.Team.Id];
            var league = leagueCache[statisticsDto.League.Id];

            var existingStatistics = player.Statistics.FirstOrDefault(statistics =>
                statistics.SeasonYear == statisticsDto.League.Season &&
                statistics.LeagueId == league.Id);

            if (existingStatistics is null)
            {
                player.Statistics.Add(
                    ApiFootballImportMapper.MapStatistics(statisticsDto, player.Id, team, league));
            }
            else
            {
                ApiFootballImportMapper.UpdateStatistics(existingStatistics, statisticsDto, team, league);
            }

            count++;
        }

        return count;
    }

    private async Task<List<T>> FetchAsync<T>(string path, CancellationToken cancellationToken)
    {
        var payload = await GetPayloadAsync<T>(path, cancellationToken);
        return payload.Response;
    }

    private async Task<List<T>> FetchAllPagesAsync<T>(
        string path,
        CancellationToken cancellationToken)
    {
        var maxPages = Math.Max(1, options.Value.MaxPages);
        var allItems = new List<T>();
        var currentPage = 1;
        var totalPages = 1;

        while (currentPage <= totalPages && currentPage <= maxPages)
        {
            // API-Football: page=1 is implicit; only page>=2 is sent explicitly.
            var requestUri = currentPage == 1 ? path : AppendQuery(path, "page", currentPage.ToString());

            var payload = await GetPayloadAsync<T>(requestUri, cancellationToken);
            allItems.AddRange(payload.Response);
            totalPages = Math.Max(payload.Paging.Total, 1);

            if (currentPage == maxPages && totalPages > maxPages)
            {
                logger.LogWarning(
                    "API-Football returned {TotalPages} pages for {Path}, but only {MaxPages} were fetched (plan limit).",
                    totalPages,
                    path,
                    maxPages);
            }

            currentPage++;
        }

        return allItems;
    }

    private async Task<ApiFootballResponseDto<T>> GetPayloadAsync<T>(
        string requestUri,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("API-Football request: {RequestUri}", requestUri);

        var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ApiFootballResponseDto<T>>(
            JsonOptions,
            cancellationToken);

        if (payload is null)
        {
            throw new InvalidOperationException("API-Football returned an empty response body.");
        }

        if (HasApiErrors(payload.Errors))
        {
            throw new InvalidOperationException($"API-Football error: {payload.Errors}");
        }

        return payload;
    }

    private static string AppendQuery(string path, string name, string value)
    {
        var separator = path.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        return $"{path}{separator}{name}={value}";
    }

    private async Task<List<ApiFootballPlayerResponseItemDto>> FetchAllPlayersFromApiAsync(
        int teamId,
        int season,
        CancellationToken cancellationToken)
    {
        var apiOptions = options.Value;
        var path = $"{apiOptions.PlayersEndpoint}?team={teamId}&season={season}";
        return await FetchAllPagesAsync<ApiFootballPlayerResponseItemDto>(path, cancellationToken);
    }

    private async Task<Dictionary<int, Team>> LoadTeamCacheAsync(CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams.ToListAsync(cancellationToken);
        return teams.ToDictionary(team => team.ExternalId);
    }

    private async Task<Dictionary<int, League>> LoadLeagueCacheAsync(CancellationToken cancellationToken)
    {
        var leagues = await dbContext.Leagues.ToListAsync(cancellationToken);
        return leagues.ToDictionary(league => league.ExternalId);
    }

    private static void ResolveTeam(
        ApiFootballTeamDto teamDto,
        Dictionary<int, Team> cache,
        List<Team> newTeams)
    {
        if (cache.ContainsKey(teamDto.Id))
        {
            ApiFootballImportMapper.UpdateTeam(cache[teamDto.Id], teamDto);
            return;
        }

        var team = ApiFootballImportMapper.MapTeam(teamDto);
        cache[teamDto.Id] = team;
        newTeams.Add(team);
    }

    private static void ResolveLeague(
        ApiFootballLeagueDto leagueDto,
        Dictionary<int, League> cache,
        List<League> newLeagues)
    {
        if (cache.ContainsKey(leagueDto.Id))
        {
            ApiFootballImportMapper.UpdateLeague(cache[leagueDto.Id], leagueDto);
            return;
        }

        var league = ApiFootballImportMapper.MapLeague(leagueDto);
        cache[leagueDto.Id] = league;
        newLeagues.Add(league);
    }

    private static bool HasApiErrors(JsonElement errors) =>
        errors.ValueKind switch
        {
            JsonValueKind.Array => errors.GetArrayLength() > 0,
            JsonValueKind.Object => errors.EnumerateObject().Any(),
            JsonValueKind.String => !string.IsNullOrWhiteSpace(errors.GetString()),
            _ => false,
        };

    private async Task RemoveLeagueSeasonDataAsync(
        int leagueId,
        int season,
        int? teamId,
        CancellationToken cancellationToken)
    {
        var playersQuery = dbContext.Players
            .Where(player =>
                player.ExternalLeagueId == leagueId &&
                (!teamId.HasValue || player.ExternalTeamId == teamId.Value));

        var players = await playersQuery
            .Include(player => player.Statistics)
            .ToListAsync(cancellationToken);

        var playersToRemove = players
            .Where(player => player.Statistics.Any(statistics => statistics.SeasonYear == season))
            .ToList();

        if (playersToRemove.Count == 0)
        {
            return;
        }

        dbContext.Players.RemoveRange(playersToRemove);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
