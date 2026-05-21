namespace ScoutNet.Domain.Entities;

public class PlayerStatistics
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid TeamId { get; set; }

    public Guid LeagueId { get; set; }

    public int SeasonYear { get; set; }

    public int? Appearances { get; set; }

    public int? Lineups { get; set; }

    public int? Minutes { get; set; }

    public int? ShirtNumber { get; set; }

    public string? Position { get; set; }

    public decimal? Rating { get; set; }

    public bool Captain { get; set; }

    public int? SubstitutesIn { get; set; }

    public int? SubstitutesOut { get; set; }

    public int? SubstitutesBench { get; set; }

    public int? ShotsTotal { get; set; }

    public int? ShotsOn { get; set; }

    public int? GoalsTotal { get; set; }

    public int? GoalsConceded { get; set; }

    public int? Assists { get; set; }

    public int? Saves { get; set; }

    public int? PassesTotal { get; set; }

    public int? KeyPasses { get; set; }

    public int? PassAccuracy { get; set; }

    public int? TacklesTotal { get; set; }

    public int? Blocks { get; set; }

    public int? Interceptions { get; set; }

    public int? DuelsTotal { get; set; }

    public int? DuelsWon { get; set; }

    public int? DribblesAttempts { get; set; }

    public int? DribblesSuccess { get; set; }

    public int? DribblesPast { get; set; }

    public int? FoulsDrawn { get; set; }

    public int? FoulsCommitted { get; set; }

    public int? YellowCards { get; set; }

    public int? RedCards { get; set; }

    public int? PenaltyWon { get; set; }

    public int? PenaltyCommitted { get; set; }

    public int? PenaltyScored { get; set; }

    public int? PenaltyMissed { get; set; }

    public int? PenaltySaved { get; set; }

    public Player Player { get; set; } = null!;

    public Team Team { get; set; } = null!;

    public League League { get; set; } = null!;
}
