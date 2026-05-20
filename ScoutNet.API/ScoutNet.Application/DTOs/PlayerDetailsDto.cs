using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.DTOs;

public class PlayerDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Nationality { get; set; } = string.Empty;

    public string CurrentClub { get; set; } = string.Empty;

    public PlayerPosition Position { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime ContractUntil { get; set; }

    public IReadOnlyList<PlayerStatisticsDto> Statistics { get; set; } = [];
}
