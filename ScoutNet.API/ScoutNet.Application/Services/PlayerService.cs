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
        int teamId,
        CancellationToken cancellationToken = default)
    {
        var hasPlayers = await playerRepository.ExistsForTeamAndSeasonAsync(
            teamId,
            season,
            cancellationToken);

        if (!hasPlayers)
        {
            await externalService.FetchAndSavePlayersAsync(
                teamId,
                season,
                cancellationToken: cancellationToken);
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
        var statistics = player.Statistics
            .Where(s => s.SeasonYear == seasonYear)
            .OrderByDescending(s => s.Appearances ?? s.Lineups ?? 0)
            .FirstOrDefault();

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
            Appearances = Delta(first.Appearances, second.Appearances),
            Lineups = Delta(first.Lineups, second.Lineups),
            Minutes = Delta(first.Minutes, second.Minutes),
            ShotsTotal = Delta(first.ShotsTotal, second.ShotsTotal),
            ShotsOn = Delta(first.ShotsOn, second.ShotsOn),
            GoalsTotal = Delta(first.GoalsTotal, second.GoalsTotal),
            GoalsConceded = Delta(first.GoalsConceded, second.GoalsConceded),
            Assists = Delta(first.Assists, second.Assists),
            Saves = Delta(first.Saves, second.Saves),
            PassesTotal = Delta(first.PassesTotal, second.PassesTotal),
            KeyPasses = Delta(first.KeyPasses, second.KeyPasses),
            PassAccuracy = Delta(first.PassAccuracy, second.PassAccuracy),
            TacklesTotal = Delta(first.TacklesTotal, second.TacklesTotal),
            Blocks = Delta(first.Blocks, second.Blocks),
            Interceptions = Delta(first.Interceptions, second.Interceptions),
            DuelsTotal = Delta(first.DuelsTotal, second.DuelsTotal),
            DuelsWon = Delta(first.DuelsWon, second.DuelsWon),
            DribblesAttempts = Delta(first.DribblesAttempts, second.DribblesAttempts),
            DribblesSuccess = Delta(first.DribblesSuccess, second.DribblesSuccess),
            DribblesPast = Delta(first.DribblesPast, second.DribblesPast),
            FoulsDrawn = Delta(first.FoulsDrawn, second.FoulsDrawn),
            FoulsCommitted = Delta(first.FoulsCommitted, second.FoulsCommitted),
            YellowCards = Delta(first.YellowCards, second.YellowCards),
            RedCards = Delta(first.RedCards, second.RedCards),
            PenaltyWon = Delta(first.PenaltyWon, second.PenaltyWon),
            PenaltyCommitted = Delta(first.PenaltyCommitted, second.PenaltyCommitted),
            PenaltyScored = Delta(first.PenaltyScored, second.PenaltyScored),
            PenaltyMissed = Delta(first.PenaltyMissed, second.PenaltyMissed),
            PenaltySaved = Delta(first.PenaltySaved, second.PenaltySaved),
        };
    }

    private static int? Delta(int? first, int? second) =>
        (second ?? 0) - (first ?? 0);
}
