using ScoutNet.Application.DTOs;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Services;

internal static class PlayerSkillsAggregator
{
    private const double MaxGoalsPerGame = 1.0;
    private const double MaxShotsOnPerGame = 0.8;
    private const double MaxAssistsPerGame = 0.6;
    private const double MaxInterceptionsPerGame = 3.0;
    private const double MaxTacklesPerGame = 5.0;

    public static PlayerSkillsDto Calculate(Player player, PlayerStatistics statistics)
    {
        var matches = Math.Max(statistics.Appearances ?? statistics.Lineups ?? 1, 1);
        var goalsPerGame = (statistics.GoalsTotal ?? 0) / (double)matches;
        var assistsPerGame = (statistics.Assists ?? 0) / (double)matches;
        var shotsOnPerGame = (statistics.ShotsOn ?? 0) / (double)matches;

        var dribbleAttempts = statistics.DribblesAttempts ?? 0;
        var dribbleSuccess = statistics.DribblesSuccess ?? 0;
        var dribbleSuccessPercentage = dribbleAttempts == 0
            ? 0
            : (double)dribbleSuccess / dribbleAttempts;

        var shooting = Blend(
            Normalize(goalsPerGame, MaxGoalsPerGame),
            Normalize(shotsOnPerGame, MaxShotsOnPerGame));

        var passing = Blend(
            (statistics.PassAccuracy ?? 0) / 100.0,
            Normalize(assistsPerGame, MaxAssistsPerGame));

        var defending = Blend(
            Normalize((statistics.Interceptions ?? 0) / (double)matches, MaxInterceptionsPerGame),
            Normalize((statistics.TacklesTotal ?? 0) / (double)matches, MaxTacklesPerGame));

        var pace = Blend(dribbleSuccessPercentage, AgeToPaceFactor(player.Age ?? 25));

        var physicality = Blend(
            Normalize((statistics.TacklesTotal ?? 0) / (double)matches, MaxTacklesPerGame),
            Normalize((statistics.Interceptions ?? 0) / (double)matches, MaxInterceptionsPerGame),
            AgeToPhysicalityFactor(player.Age ?? 25));

        return new PlayerSkillsDto
        {
            Pace = ToRating(pace),
            Shooting = ToRating(shooting),
            Passing = ToRating(passing),
            Dribbling = ToRating(dribbleSuccessPercentage),
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

    private static double Blend(params double[] values) =>
        values.Length == 0 ? 0 : values.Average();

    private static int ToRating(double normalized) =>
        (int)Math.Clamp(Math.Round(normalized * 100), 1, 100);
}
