using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.DTOs;

public class PlayerFilterDto
{
    public string? SearchTerm { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public PlayerPosition? Position { get; set; }

    public string? Nationality { get; set; }

    public DateTime? ContractUntilFrom { get; set; }

    public DateTime? ContractUntilTo { get; set; }

    public int? MinMatchesPlayed { get; set; }

    public int? MaxMatchesPlayed { get; set; }

    public int? MinGoals { get; set; }

    public int? MinAssists { get; set; }

    public double? MinExpectedGoals { get; set; }

    public double? MinPassAccuracyPercentage { get; set; }

    public double? MinDribblesSuccessPercentage { get; set; }

    public double? MinInterceptionsPerGame { get; set; }

    public double? MinTacklesPerGame { get; set; }
}
