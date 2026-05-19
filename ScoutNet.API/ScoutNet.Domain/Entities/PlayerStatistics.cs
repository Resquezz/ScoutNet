namespace ScoutNet.Domain.Entities;

public class PlayerStatistics
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public int MatchesPlayed { get; set; }

    public int Goals { get; set; }

    public int Assists { get; set; }

    public double ExpectedGoals { get; set; }

    public double PassAccuracyPercentage { get; set; }

    public double DribblesSuccessPercentage { get; set; }

    public double InterceptionsPerGame { get; set; }

    public double TacklesPerGame { get; set; }

    public Player Player { get; set; } = null!;
}
