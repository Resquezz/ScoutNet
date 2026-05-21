using System.Linq.Expressions;
using ScoutNet.Application.DTOs;
using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class PlayerFilterSpecification : BaseSpecification<Player>
{
    public PlayerFilterSpecification(PlayerFilterDto filter, int leagueId, int seasonYear)
        : base(BuildCriteria(filter, leagueId, seasonYear))
    {
        AddInclude(player => player.TeamProfile);
        AddInclude(player => player.LeagueProfile);
        AddInclude(player => player.Statistics);
        AddIncludeChain("Statistics.Team");
        AddIncludeChain("Statistics.League");
    }

    private static Expression<Func<Player, bool>> BuildCriteria(
        PlayerFilterDto filter,
        int leagueId,
        int seasonYear)
    {
        return player =>
            player.ExternalLeagueId == leagueId &&
            player.Statistics.Any(statistics =>
                statistics.SeasonYear == seasonYear &&
                (!filter.MinAppearances.HasValue ||
                 (statistics.Appearances ?? 0) >= filter.MinAppearances.Value) &&
                (!filter.MaxAppearances.HasValue ||
                 (statistics.Appearances ?? 0) <= filter.MaxAppearances.Value) &&
                (!filter.MinGoals.HasValue || (statistics.GoalsTotal ?? 0) >= filter.MinGoals.Value) &&
                (!filter.MinAssists.HasValue || (statistics.Assists ?? 0) >= filter.MinAssists.Value) &&
                (!filter.MinShotsOn.HasValue || (statistics.ShotsOn ?? 0) >= filter.MinShotsOn.Value) &&
                (!filter.MinPassAccuracy.HasValue ||
                 (statistics.PassAccuracy ?? 0) >= filter.MinPassAccuracy.Value) &&
                (!filter.MinDribblesSuccess.HasValue ||
                 (statistics.DribblesSuccess ?? 0) >= filter.MinDribblesSuccess.Value) &&
                (!filter.MinInterceptions.HasValue ||
                 (statistics.Interceptions ?? 0) >= filter.MinInterceptions.Value) &&
                (!filter.MinTackles.HasValue ||
                 (statistics.TacklesTotal ?? 0) >= filter.MinTackles.Value)) &&
            (!filter.MinAge.HasValue || (player.Age ?? 0) >= filter.MinAge.Value) &&
            (!filter.MaxAge.HasValue || (player.Age ?? 0) <= filter.MaxAge.Value) &&
            (filter.Position == null || player.Position == filter.Position) &&
            (string.IsNullOrWhiteSpace(filter.Nationality) || player.Nationality == filter.Nationality) &&
            (string.IsNullOrWhiteSpace(filter.SearchTerm) ||
             player.Name.ToLower().Contains(filter.SearchTerm.ToLower()));
    }
}
