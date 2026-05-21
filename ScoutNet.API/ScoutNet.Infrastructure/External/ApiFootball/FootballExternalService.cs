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
    IHttpClientFactory httpClientFactory,
    ScoutDbContext dbContext,
    IOptions<ApiFootballOptions> options,
    ILogger<FootballExternalService> logger) : IFootballExternalService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task FetchAndSavePlayersAsync(
        int teamId,
        int season,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (forceRefresh)
        {
            await RemoveTeamSeasonDataAsync(teamId, season, cancellationToken);
        }
        else if (await dbContext.Players.AnyAsync(
                     player => player.ExternalTeamId == teamId &&
                               player.Statistics.Any(statistics => statistics.SeasonYear == season),
                     cancellationToken))
        {
            logger.LogInformation(
                "Players for team {TeamId} and season {Season} already exist. Skipping API import.",
                teamId,
                season);
            return;
        }

        var apiItems = await FetchAllPlayersFromApiAsync(teamId, season, cancellationToken);
        var teamCache = await LoadTeamCacheAsync(cancellationToken);
        var leagueCache = await LoadLeagueCacheAsync(cancellationToken);

        var newTeams = new List<Team>();
        var newLeagues = new List<League>();
        var players = new List<Player>();

        foreach (var item in apiItems)
        {
            var teamStatistics = item.Statistics
                .Where(statistics =>
                    statistics.Team.Id == teamId &&
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

            var player = ApiFootballImportMapper.MapPlayer(item, primaryStatistics, team, league, teamId);
            player.Statistics = teamStatistics
                .Select(statisticsDto =>
                    ApiFootballImportMapper.MapStatistics(
                        statisticsDto,
                        player.Id,
                        teamCache[statisticsDto.Team.Id],
                        leagueCache[statisticsDto.League.Id]))
                .ToList();

            players.Add(player);
        }

        if (players.Count == 0)
        {
            logger.LogWarning(
                "API-Football returned no mappable players for team {TeamId}, season {Season}.",
                teamId,
                season);
            return;
        }

        if (newTeams.Count > 0)
        {
            await dbContext.Teams.AddRangeAsync(newTeams, cancellationToken);
        }

        if (newLeagues.Count > 0)
        {
            await dbContext.Leagues.AddRangeAsync(newLeagues, cancellationToken);
        }

        await dbContext.Players.AddRangeAsync(players, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var statisticsCount = players.Sum(player => player.Statistics.Count);

        logger.LogInformation(
            "Imported {TeamCount} teams, {LeagueCount} leagues, {PlayerCount} players, {StatisticsCount} statistics for team {TeamId}, season {Season}.",
            newTeams.Count,
            newLeagues.Count,
            players.Count,
            statisticsCount,
            teamId,
            season);
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

    private async Task<List<ApiFootballPlayerResponseItemDto>> FetchAllPlayersFromApiAsync(
        int teamId,
        int season,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(ApiFootballHttpClient.Name);
        var apiOptions = options.Value;
        var allItems = new List<ApiFootballPlayerResponseItemDto>();
        var currentPage = 1;
        var totalPages = 1;

        while (currentPage <= totalPages)
        {
            var requestUri =
                $"{apiOptions.PlayersEndpoint}?team={teamId}&season={season}&page={currentPage}";

            logger.LogDebug("API-Football request: {RequestUri}", requestUri);

            var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<ApiFootballResponseDto<ApiFootballPlayerResponseItemDto>>(
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

            allItems.AddRange(payload.Response);
            totalPages = Math.Max(payload.Paging.Total, 1);
            currentPage++;
        }

        return allItems;
    }

    private static bool HasApiErrors(JsonElement errors) =>
        errors.ValueKind switch
        {
            JsonValueKind.Array => errors.GetArrayLength() > 0,
            JsonValueKind.Object => errors.EnumerateObject().Any(),
            JsonValueKind.String => !string.IsNullOrWhiteSpace(errors.GetString()),
            _ => false,
        };

    private async Task RemoveTeamSeasonDataAsync(
        int teamId,
        int season,
        CancellationToken cancellationToken)
    {
        var players = await dbContext.Players
            .Where(player => player.ExternalTeamId == teamId)
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
