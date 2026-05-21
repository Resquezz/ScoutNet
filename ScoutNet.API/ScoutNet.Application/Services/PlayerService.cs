using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces.External;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.Application.Mapping;
using ScoutNet.Application.Specifications;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Services;

public class PlayerService(
    IPlayerRepository playerRepository,
    IFootballExternalService externalService) : IPlayerService
{
    public async Task<IReadOnlyList<PlayerDto>> GetPlayersWithSyncAsync(
        PlayerFilterDto filter,
        int season,
        int leagueId,
        CancellationToken cancellationToken = default)
    {
        var hasPlayers = await playerRepository.ExistsForLeagueAndSeasonAsync(
            leagueId,
            season,
            cancellationToken);

        if (!hasPlayers)
        {
            await externalService.FetchAndSavePlayersAsync(leagueId, season, cancellationToken);
        }

        var players = await playerRepository.ListBySpecAsync(
            new PlayerFilterSpecification(filter, leagueId, season),
            cancellationToken);

        return players.Select(PlayerMapper.ToDto).ToList();
    }

    public async Task<PlayerComparisonDto> ComparePlayersAsync(
        int id1,
        int id2,
        int season,
        CancellationToken cancellationToken = default)
    {
        var player1 = await LoadPlayerOrThrowAsync(id1, cancellationToken);
        var player2 = await LoadPlayerOrThrowAsync(id2, cancellationToken);

        var stats1 = GetSeasonStatisticsOrThrow(player1, season);
        var stats2 = GetSeasonStatisticsOrThrow(player2, season);

        var statsDto1 = PlayerMapper.ToStatisticsDto(stats1);
        var statsDto2 = PlayerMapper.ToStatisticsDto(stats2);

        return new PlayerComparisonDto
        {
            SeasonYear = season,
            Players =
            [
                BuildComparisonEntry(player1, statsDto1, stats1),
                BuildComparisonEntry(player2, statsDto2, stats2),
            ],
            MetricsDelta = CalculateDelta(statsDto1, statsDto2),
        };
    }

    private async Task<Player> LoadPlayerOrThrowAsync(int externalId, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetBySpecAsync(
            new PlayerByExternalIdWithStatisticsSpecification(externalId),
            cancellationToken);

        if (player is null)
        {
            throw new KeyNotFoundException($"Player with id '{externalId}' was not found.");
        }

        return player;
    }

    private static PlayerStatistics GetSeasonStatisticsOrThrow(Player player, int seasonYear)
    {
        var statistics = player.Statistics.FirstOrDefault(s => s.SeasonYear == seasonYear);

        if (statistics is null)
        {
            throw new KeyNotFoundException(
                $"Statistics for season '{seasonYear}' were not found for player '{player.ExternalId}'.");
        }

        return statistics;
    }

    private static PlayerComparisonDto.Entry BuildComparisonEntry(
        Player player,
        PlayerStatisticsDto statisticsDto,
        PlayerStatistics statistics)
    {
        return new PlayerComparisonDto.Entry
        {
            Profile = PlayerMapper.ToDto(player),
            SeasonStatistics = statisticsDto,
            Skills = PlayerSkillsAggregator.Calculate(player, statistics),
        };
    }

    private static PlayerStatisticsDeltaDto CalculateDelta(
        PlayerStatisticsDto first,
        PlayerStatisticsDto second)
    {
        return new PlayerStatisticsDeltaDto
        {
            MatchesPlayed = second.MatchesPlayed - first.MatchesPlayed,
            Goals = second.Goals - first.Goals,
            Assists = second.Assists - first.Assists,
            ExpectedGoals = second.ExpectedGoals - first.ExpectedGoals,
            PassAccuracyPercentage = second.PassAccuracyPercentage - first.PassAccuracyPercentage,
            DribblesSuccessPercentage = second.DribblesSuccessPercentage - first.DribblesSuccessPercentage,
            InterceptionsPerGame = second.InterceptionsPerGame - first.InterceptionsPerGame,
            TacklesPerGame = second.TacklesPerGame - first.TacklesPerGame,
        };
    }
}
