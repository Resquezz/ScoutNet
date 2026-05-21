using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.DTOs;

public class PlayerDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public int? Age { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? BirthPlace { get; set; }

    public string? BirthCountry { get; set; }

    public string Nationality { get; set; } = string.Empty;

    public string? Height { get; set; }

    public string? Weight { get; set; }

    public bool Injured { get; set; }

    public string? PhotoUrl { get; set; }

    public string CurrentClub { get; set; } = string.Empty;

    public PlayerPosition Position { get; set; }

    public TeamDto Team { get; set; } = new();

    public LeagueDto League { get; set; } = new();

    public IReadOnlyList<PlayerStatisticsDto> Statistics { get; set; } = [];
}
