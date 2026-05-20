namespace ScoutNet.Application.DTOs;

public class PlayerStatisticsDto
{
    public Guid Id { get; set; }

    public string Season { get; set; } = string.Empty;

    public int MatchesPlayed { get; set; }

    public int Goals { get; set; }

    public int Assists { get; set; }

    public double ExpectedGoals { get; set; }

    public double PassAccuracyPercentage { get; set; }

    public double DribblesSuccessPercentage { get; set; }

    public double InterceptionsPerGame { get; set; }

    public double TacklesPerGame { get; set; }
}
