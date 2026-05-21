using ScoutNet.Application.DTOs;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Mapping;

internal static class PlayerMapper
{
    public static PlayerDto ToDto(Player player) => new()
    {
        Id = player.ExternalId,
        Name = player.Name,
        Firstname = player.Firstname,
        Lastname = player.Lastname,
        Age = player.Age,
        BirthDate = player.BirthDate,
        BirthPlace = player.BirthPlace,
        BirthCountry = player.BirthCountry,
        Nationality = player.Nationality,
        Height = player.Height,
        Weight = player.Weight,
        Injured = player.Injured,
        PhotoUrl = player.PhotoUrl,
        CurrentClub = player.CurrentClub,
        Position = player.Position,
        Team = ToTeamDto(player.TeamProfile),
        League = ToLeagueDto(player.LeagueProfile),
    };

    public static PlayerDetailsDto ToDetailsDto(Player player) => new()
    {
        Id = player.ExternalId,
        Name = player.Name,
        Firstname = player.Firstname,
        Lastname = player.Lastname,
        Age = player.Age,
        BirthDate = player.BirthDate,
        BirthPlace = player.BirthPlace,
        BirthCountry = player.BirthCountry,
        Nationality = player.Nationality,
        Height = player.Height,
        Weight = player.Weight,
        Injured = player.Injured,
        PhotoUrl = player.PhotoUrl,
        CurrentClub = player.CurrentClub,
        Position = player.Position,
        Team = ToTeamDto(player.TeamProfile),
        League = ToLeagueDto(player.LeagueProfile),
        Statistics = player.Statistics
            .OrderByDescending(statistics => statistics.SeasonYear)
            .Select(ToStatisticsDto)
            .ToList(),
    };

    public static PlayerStatisticsDto ToStatisticsDto(PlayerStatistics statistics) => new()
    {
        Id = statistics.Id,
        SeasonYear = statistics.SeasonYear,
        Season = FormatSeason(statistics.SeasonYear),
        Team = ToTeamDto(statistics.Team),
        League = ToLeagueDto(statistics.League),
        Appearances = statistics.Appearances,
        Lineups = statistics.Lineups,
        Minutes = statistics.Minutes,
        ShirtNumber = statistics.ShirtNumber,
        Position = statistics.Position,
        Rating = statistics.Rating,
        Captain = statistics.Captain,
        SubstitutesIn = statistics.SubstitutesIn,
        SubstitutesOut = statistics.SubstitutesOut,
        SubstitutesBench = statistics.SubstitutesBench,
        ShotsTotal = statistics.ShotsTotal,
        ShotsOn = statistics.ShotsOn,
        GoalsTotal = statistics.GoalsTotal,
        GoalsConceded = statistics.GoalsConceded,
        Assists = statistics.Assists,
        Saves = statistics.Saves,
        PassesTotal = statistics.PassesTotal,
        KeyPasses = statistics.KeyPasses,
        PassAccuracy = statistics.PassAccuracy,
        TacklesTotal = statistics.TacklesTotal,
        Blocks = statistics.Blocks,
        Interceptions = statistics.Interceptions,
        DuelsTotal = statistics.DuelsTotal,
        DuelsWon = statistics.DuelsWon,
        DribblesAttempts = statistics.DribblesAttempts,
        DribblesSuccess = statistics.DribblesSuccess,
        DribblesPast = statistics.DribblesPast,
        FoulsDrawn = statistics.FoulsDrawn,
        FoulsCommitted = statistics.FoulsCommitted,
        YellowCards = statistics.YellowCards,
        RedCards = statistics.RedCards,
        PenaltyWon = statistics.PenaltyWon,
        PenaltyCommitted = statistics.PenaltyCommitted,
        PenaltyScored = statistics.PenaltyScored,
        PenaltyMissed = statistics.PenaltyMissed,
        PenaltySaved = statistics.PenaltySaved,
    };

    public static TeamDto ToTeamDto(Team team) => new()
    {
        ExternalId = team.ExternalId,
        Name = team.Name,
        Logo = team.Logo,
    };

    public static LeagueDto ToLeagueDto(League league) => new()
    {
        ExternalId = league.ExternalId,
        Name = league.Name,
        Country = league.Country,
        Logo = league.Logo,
        Flag = league.Flag,
    };

    public static string FormatSeason(int seasonYear) =>
        $"{seasonYear}/{(seasonYear + 1) % 100:D2}";
}
