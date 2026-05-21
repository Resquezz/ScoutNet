using ScoutNet.Domain.Enums;

namespace ScoutNet.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }

    public int ExternalId { get; set; }

    public int ExternalTeamId { get; set; }

    public int ExternalLeagueId { get; set; }

    public Guid TeamProfileId { get; set; }

    public Guid LeagueProfileId { get; set; }

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

    public Team TeamProfile { get; set; } = null!;

    public League LeagueProfile { get; set; } = null!;

    public ICollection<PlayerStatistics> Statistics { get; set; } = [];

    public ICollection<Watchlist> Watchlists { get; set; } = [];

    public ICollection<ScoutReport> ScoutReports { get; set; } = [];
}
