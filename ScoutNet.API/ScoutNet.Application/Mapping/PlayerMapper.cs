using ScoutNet.Application.DTOs;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Mapping;

internal static class PlayerMapper
{
    public static PlayerDto ToDto(Player player) => new()
    {
        Id = player.ExternalId,
        Name = player.Name,
        Age = player.Age,
        Nationality = player.Nationality,
        CurrentClub = player.CurrentClub,
        Position = player.Position,
        PhotoUrl = player.PhotoUrl,
        ContractUntil = player.ContractUntil,
    };

    public static PlayerDetailsDto ToDetailsDto(Player player) => new()
    {
        Id = player.ExternalId,
        Name = player.Name,
        Age = player.Age,
        Nationality = player.Nationality,
        CurrentClub = player.CurrentClub,
        Position = player.Position,
        PhotoUrl = player.PhotoUrl,
        ContractUntil = player.ContractUntil,
        Statistics = player.Statistics
            .OrderByDescending(s => s.SeasonYear)
            .Select(ToStatisticsDto)
            .ToList(),
    };

    public static PlayerStatisticsDto ToStatisticsDto(PlayerStatistics statistics) => new()
    {
        Id = statistics.Id,
        Season = FormatSeason(statistics.SeasonYear),
        MatchesPlayed = statistics.MatchesPlayed,
        Goals = statistics.Goals,
        Assists = statistics.Assists,
        ExpectedGoals = statistics.ExpectedGoals,
        PassAccuracyPercentage = statistics.PassAccuracyPercentage,
        DribblesSuccessPercentage = statistics.DribblesSuccessPercentage,
        InterceptionsPerGame = statistics.InterceptionsPerGame,
        TacklesPerGame = statistics.TacklesPerGame,
    };

    public static string FormatSeason(int seasonYear) =>
        $"{seasonYear}/{(seasonYear + 1) % 100:D2}";
}
