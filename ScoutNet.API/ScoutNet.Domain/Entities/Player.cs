using ScoutNet.Domain.Enums;

namespace ScoutNet.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Nationality { get; set; } = string.Empty;

    public string CurrentClub { get; set; } = string.Empty;

    public PlayerPosition Position { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime ContractUntil { get; set; }

    public PlayerStatistics? Statistics { get; set; }

    public ICollection<Watchlist> Watchlists { get; set; } = [];

    public ICollection<ScoutReport> ScoutReports { get; set; } = [];
}
