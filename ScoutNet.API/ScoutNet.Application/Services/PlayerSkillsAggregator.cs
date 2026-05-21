using ScoutNet.Application.DTOs;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Services;

internal static class PlayerSkillsAggregator
{
    private const double MaxGoalsPerGame = 1.0;
    private const double MaxExpectedGoalsPerGame = 0.9;
    private const double MaxAssistsPerGame = 0.6;
    private const double MaxInterceptionsPerGame = 3.0;
    private const double MaxTacklesPerGame = 5.0;

    public static PlayerSkillsDto Calculate(Player player, PlayerStatistics statistics)
    {
        var matches = Math.Max(statistics.MatchesPlayed, 1);
        var goalsPerGame = statistics.Goals / (double)matches;
        var assistsPerGame = statistics.Assists / (double)matches;
        var expectedGoalsPerGame = statistics.ExpectedGoals / matches;

        var shooting = Blend(
            Normalize(goalsPerGame, MaxGoalsPerGame),
            Normalize(expectedGoalsPerGame, MaxExpectedGoalsPerGame));

        var passing = Blend(
            statistics.PassAccuracyPercentage / 100.0,
            Normalize(assistsPerGame, MaxAssistsPerGame));

        var dribbling = statistics.DribblesSuccessPercentage / 100.0;

        var defending = Blend(
            Normalize(statistics.InterceptionsPerGame, MaxInterceptionsPerGame),
            Normalize(statistics.TacklesPerGame, MaxTacklesPerGame));

        var pace = Blend(
            statistics.DribblesSuccessPercentage / 100.0,
            AgeToPaceFactor(player.Age));

        var physicality = Blend(
            Normalize(statistics.TacklesPerGame, MaxTacklesPerGame),
            Normalize(statistics.InterceptionsPerGame, MaxInterceptionsPerGame),
            AgeToPhysicalityFactor(player.Age));

        return new PlayerSkillsDto
        {
            Pace = ToRating(pace),
            Shooting = ToRating(shooting),
            Passing = ToRating(passing),
            Dribbling = ToRating(dribbling),
            Defending = ToRating(defending),
            Physicality = ToRating(physicality),
        };
    }

    private static double AgeToPaceFactor(int age)
    {
        const int peakAge = 22;
        const int minAge = 16;
        const int maxAge = 36;

        if (age <= peakAge)
        {
            return Normalize(age - minAge, peakAge - minAge);
        }

        return 1.0 - Normalize(age - peakAge, maxAge - peakAge) * 0.35;
    }

    private static double AgeToPhysicalityFactor(int age)
    {
        const int peakAge = 27;
        const double spread = 8.0;
        var distance = Math.Abs(age - peakAge);
        return Math.Clamp(1.0 - distance / spread, 0.55, 1.0);
    }

    private static double Normalize(double value, double max) =>
        max <= 0 ? 0 : Math.Clamp(value / max, 0, 1);

    private static double Blend(params double[] values)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        return values.Average();
    }

    private static int ToRating(double normalized) =>
        (int)Math.Clamp(Math.Round(normalized * 100), 1, 100);
}
