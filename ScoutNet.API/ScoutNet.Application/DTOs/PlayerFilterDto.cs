using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.DTOs;

public class PlayerFilterDto
{
    public string? SearchTerm { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public PlayerPosition? Position { get; set; }

    public string? Nationality { get; set; }

    public int? MinAppearances { get; set; }

    public int? MaxAppearances { get; set; }

    public int? MinGoals { get; set; }

    public int? MinAssists { get; set; }

    public int? MinShotsOn { get; set; }

    public int? MinPassAccuracy { get; set; }

    public int? MinDribblesSuccess { get; set; }

    public int? MinInterceptions { get; set; }

    public int? MinTackles { get; set; }
}
