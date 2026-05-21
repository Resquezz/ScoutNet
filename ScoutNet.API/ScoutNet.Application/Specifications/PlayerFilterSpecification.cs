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
        AddInclude(player => player.Statistics);
    }

    private static Expression<Func<Player, bool>> BuildCriteria(
        PlayerFilterDto filter,
        int leagueId,
        int seasonYear)
    {
        return player =>
            player.LeagueId == leagueId &&
            player.Statistics.Any(statistics =>
                statistics.SeasonYear == seasonYear &&
                (!filter.MinMatchesPlayed.HasValue || statistics.MatchesPlayed >= filter.MinMatchesPlayed.Value) &&
                (!filter.MaxMatchesPlayed.HasValue || statistics.MatchesPlayed <= filter.MaxMatchesPlayed.Value) &&
                (!filter.MinGoals.HasValue || statistics.Goals >= filter.MinGoals.Value) &&
                (!filter.MinAssists.HasValue || statistics.Assists >= filter.MinAssists.Value) &&
                (!filter.MinExpectedGoals.HasValue || statistics.ExpectedGoals >= filter.MinExpectedGoals.Value) &&
                (!filter.MinPassAccuracyPercentage.HasValue ||
                 statistics.PassAccuracyPercentage >= filter.MinPassAccuracyPercentage.Value) &&
                (!filter.MinDribblesSuccessPercentage.HasValue ||
                 statistics.DribblesSuccessPercentage >= filter.MinDribblesSuccessPercentage.Value) &&
                (!filter.MinInterceptionsPerGame.HasValue ||
                 statistics.InterceptionsPerGame >= filter.MinInterceptionsPerGame.Value) &&
                (!filter.MinTacklesPerGame.HasValue ||
                 statistics.TacklesPerGame >= filter.MinTacklesPerGame.Value)) &&
            (!filter.MinAge.HasValue || player.Age >= filter.MinAge.Value) &&
            (!filter.MaxAge.HasValue || player.Age <= filter.MaxAge.Value) &&
            (filter.Position == null || player.Position == filter.Position) &&
            (string.IsNullOrWhiteSpace(filter.Nationality) || player.Nationality == filter.Nationality) &&
            (string.IsNullOrWhiteSpace(filter.SearchTerm) ||
             player.Name.ToLower().Contains(filter.SearchTerm.ToLower())) &&
            (!filter.ContractUntilFrom.HasValue || player.ContractUntil >= filter.ContractUntilFrom.Value) &&
            (!filter.ContractUntilTo.HasValue || player.ContractUntil <= filter.ContractUntilTo.Value);
    }
}
