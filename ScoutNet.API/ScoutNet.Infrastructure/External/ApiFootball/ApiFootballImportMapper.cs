using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Enums;
using ScoutNet.Infrastructure.External.ApiFootball.Models;

namespace ScoutNet.Infrastructure.External.ApiFootball;

internal static class ApiFootballImportMapper
{
    public static Team MapTeam(ApiFootballTeamDto team) => new()
    {
        Id = Guid.NewGuid(),
        ExternalId = team.Id,
        Name = team.Name,
        Logo = team.Logo,
    };

    public static void UpdateTeam(Team entity, ApiFootballTeamDto team)
    {
        entity.Name = team.Name;
        entity.Logo = team.Logo;
    }

    public static League MapLeague(ApiFootballLeagueDto league) => new()
    {
        Id = Guid.NewGuid(),
        ExternalId = league.Id,
        Name = league.Name,
        Country = league.Country,
        Logo = league.Logo,
        Flag = league.Flag,
    };

    public static void UpdateLeague(League entity, ApiFootballLeagueDto league)
    {
        entity.Name = league.Name;
        entity.Country = league.Country;
        entity.Logo = league.Logo;
        entity.Flag = league.Flag;
    }

    public static Player MapPlayer(
        ApiFootballPlayerResponseItemDto item,
        ApiFootballPlayerStatisticsDto primaryStatistics,
        Team team,
        League league,
        int externalTeamId)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            ExternalId = item.Player.Id,
            ExternalTeamId = externalTeamId,
            ExternalLeagueId = league.ExternalId,
            TeamProfileId = team.Id,
            LeagueProfileId = league.Id,
            Name = item.Player.Name,
            Firstname = item.Player.Firstname,
            Lastname = item.Player.Lastname,
            Age = item.Player.Age,
            BirthDate = ParseBirthDate(item.Player.Birth?.Date),
            BirthPlace = item.Player.Birth?.Place,
            BirthCountry = item.Player.Birth?.Country,
            Nationality = item.Player.Nationality ?? string.Empty,
            Height = item.Player.Height,
            Weight = item.Player.Weight,
            Injured = item.Player.Injured,
            PhotoUrl = item.Player.Photo,
            CurrentClub = primaryStatistics.Team.Name,
            Position = MapPosition(primaryStatistics.Games.Position),
        };
    }

    public static PlayerStatistics MapStatistics(
        ApiFootballPlayerStatisticsDto statistics,
        Guid playerId,
        Team team,
        League league)
    {
        return new PlayerStatistics
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            TeamId = team.Id,
            LeagueId = league.Id,
            SeasonYear = statistics.League.Season,
            Appearances = statistics.Games.Appearences,
            Lineups = statistics.Games.Lineups,
            Minutes = statistics.Games.Minutes,
            ShirtNumber = statistics.Games.Number,
            Position = statistics.Games.Position,
            Rating = ParseRating(statistics.Games.Rating),
            Captain = statistics.Games.Captain,
            SubstitutesIn = statistics.Substitutes?.In,
            SubstitutesOut = statistics.Substitutes?.Out,
            SubstitutesBench = statistics.Substitutes?.Bench,
            ShotsTotal = statistics.Shots?.Total,
            ShotsOn = statistics.Shots?.On,
            GoalsTotal = statistics.Goals?.Total,
            GoalsConceded = statistics.Goals?.Conceded,
            Assists = statistics.Goals?.Assists,
            Saves = statistics.Goals?.Saves,
            PassesTotal = statistics.Passes?.Total,
            KeyPasses = statistics.Passes?.Key,
            PassAccuracy = statistics.Passes?.Accuracy,
            TacklesTotal = statistics.Tackles?.Total,
            Blocks = statistics.Tackles?.Blocks,
            Interceptions = statistics.Tackles?.Interceptions,
            DuelsTotal = statistics.Duels?.Total,
            DuelsWon = statistics.Duels?.Won,
            DribblesAttempts = statistics.Dribbles?.Attempts,
            DribblesSuccess = statistics.Dribbles?.Success,
            DribblesPast = statistics.Dribbles?.Past,
            FoulsDrawn = statistics.Fouls?.Drawn,
            FoulsCommitted = statistics.Fouls?.Committed,
            YellowCards = statistics.Cards?.Yellow,
            RedCards = statistics.Cards?.Red,
            PenaltyWon = statistics.Penalty?.Won,
            PenaltyCommitted = statistics.Penalty?.Commited,
            PenaltyScored = statistics.Penalty?.Scored,
            PenaltyMissed = statistics.Penalty?.Missed,
            PenaltySaved = statistics.Penalty?.Saved,
        };
    }

    private static DateOnly? ParseBirthDate(string? value) =>
        DateOnly.TryParse(value, out var date) ? date : null;

    private static decimal? ParseRating(string? value) =>
        decimal.TryParse(value, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var rating)
            ? rating
            : null;

    private static PlayerPosition MapPosition(string? apiPosition) =>
        apiPosition?.ToLowerInvariant() switch
        {
            "goalkeeper" => PlayerPosition.GK,
            "defender" => PlayerPosition.CB,
            "midfielder" => PlayerPosition.CM,
            "attacker" => PlayerPosition.ST,
            _ => PlayerPosition.CM,
        };
}
