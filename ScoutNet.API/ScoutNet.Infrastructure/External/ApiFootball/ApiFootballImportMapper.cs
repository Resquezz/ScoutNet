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

    public static League MapLeague(ApiFootballLeagueInfoDto league, ApiFootballCountryDto? country) => new()
    {
        Id = Guid.NewGuid(),
        ExternalId = league.Id,
        Name = league.Name,
        Country = country?.Name,
        Logo = league.Logo,
        Flag = country?.Flag,
    };

    public static void UpdateLeague(League entity, ApiFootballLeagueDto league)
    {
        entity.Name = league.Name;
        entity.Country = league.Country;
        entity.Logo = league.Logo;
        entity.Flag = league.Flag;
    }

    public static void UpdateLeague(League entity, ApiFootballLeagueInfoDto league, ApiFootballCountryDto? country)
    {
        entity.Name = league.Name;
        entity.Country = country?.Name;
        entity.Logo = league.Logo;
        entity.Flag = country?.Flag;
    }

    public static Player MapPlayer(
        ApiFootballPlayerResponseItemDto item,
        ApiFootballPlayerStatisticsDto primaryStatistics,
        Team team,
        League league,
        int externalTeamId)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            ExternalId = item.Player.Id,
        };

        ApplyPlayerProfile(player, item, primaryStatistics, team, league, externalTeamId);
        return player;
    }

    public static void UpdatePlayer(
        Player player,
        ApiFootballPlayerResponseItemDto item,
        ApiFootballPlayerStatisticsDto primaryStatistics,
        Team team,
        League league,
        int externalTeamId) =>
        ApplyPlayerProfile(player, item, primaryStatistics, team, league, externalTeamId);

    private static void ApplyPlayerProfile(
        Player player,
        ApiFootballPlayerResponseItemDto item,
        ApiFootballPlayerStatisticsDto primaryStatistics,
        Team team,
        League league,
        int externalTeamId)
    {
        player.ExternalTeamId = externalTeamId;
        player.ExternalLeagueId = league.ExternalId;
        player.TeamProfileId = team.Id;
        player.LeagueProfileId = league.Id;
        player.Name = item.Player.Name;
        player.Firstname = item.Player.Firstname;
        player.Lastname = item.Player.Lastname;
        player.Age = item.Player.Age;
        player.BirthDate = ParseBirthDate(item.Player.Birth?.Date);
        player.BirthPlace = item.Player.Birth?.Place;
        player.BirthCountry = item.Player.Birth?.Country;
        player.Nationality = item.Player.Nationality ?? string.Empty;
        player.Height = item.Player.Height;
        player.Weight = item.Player.Weight;
        player.Injured = item.Player.Injured;
        player.PhotoUrl = item.Player.Photo;
        player.CurrentClub = primaryStatistics.Team.Name;
        player.Position = MapPosition(primaryStatistics.Games.Position);
    }

    public static PlayerStatistics MapStatistics(
        ApiFootballPlayerStatisticsDto statistics,
        Guid playerId,
        Team team,
        League league)
    {
        var entity = new PlayerStatistics
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
        };

        ApplyStatistics(entity, statistics, team, league);
        return entity;
    }

    public static void UpdateStatistics(
        PlayerStatistics entity,
        ApiFootballPlayerStatisticsDto statistics,
        Team team,
        League league) =>
        ApplyStatistics(entity, statistics, team, league);

    private static void ApplyStatistics(
        PlayerStatistics entity,
        ApiFootballPlayerStatisticsDto statistics,
        Team team,
        League league)
    {
        entity.TeamId = team.Id;
        entity.LeagueId = league.Id;
        entity.SeasonYear = statistics.League.Season;
        entity.Appearances = statistics.Games.Appearences;
        entity.Lineups = statistics.Games.Lineups;
        entity.Minutes = statistics.Games.Minutes;
        entity.ShirtNumber = statistics.Games.Number;
        entity.Position = statistics.Games.Position;
        entity.Rating = ParseRating(statistics.Games.Rating);
        entity.Captain = statistics.Games.Captain;
        entity.SubstitutesIn = statistics.Substitutes?.In;
        entity.SubstitutesOut = statistics.Substitutes?.Out;
        entity.SubstitutesBench = statistics.Substitutes?.Bench;
        entity.ShotsTotal = statistics.Shots?.Total;
        entity.ShotsOn = statistics.Shots?.On;
        entity.GoalsTotal = statistics.Goals?.Total;
        entity.GoalsConceded = statistics.Goals?.Conceded;
        entity.Assists = statistics.Goals?.Assists;
        entity.Saves = statistics.Goals?.Saves;
        entity.PassesTotal = statistics.Passes?.Total;
        entity.KeyPasses = statistics.Passes?.Key;
        entity.PassAccuracy = statistics.Passes?.Accuracy;
        entity.TacklesTotal = statistics.Tackles?.Total;
        entity.Blocks = statistics.Tackles?.Blocks;
        entity.Interceptions = statistics.Tackles?.Interceptions;
        entity.DuelsTotal = statistics.Duels?.Total;
        entity.DuelsWon = statistics.Duels?.Won;
        entity.DribblesAttempts = statistics.Dribbles?.Attempts;
        entity.DribblesSuccess = statistics.Dribbles?.Success;
        entity.DribblesPast = statistics.Dribbles?.Past;
        entity.FoulsDrawn = statistics.Fouls?.Drawn;
        entity.FoulsCommitted = statistics.Fouls?.Committed;
        entity.YellowCards = statistics.Cards?.Yellow;
        entity.RedCards = statistics.Cards?.Red;
        entity.PenaltyWon = statistics.Penalty?.Won;
        entity.PenaltyCommitted = statistics.Penalty?.Commited;
        entity.PenaltyScored = statistics.Penalty?.Scored;
        entity.PenaltyMissed = statistics.Penalty?.Missed;
        entity.PenaltySaved = statistics.Penalty?.Saved;
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
