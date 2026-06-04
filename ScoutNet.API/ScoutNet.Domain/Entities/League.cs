namespace ScoutNet.Domain.Entities;

public class League
{
    public Guid Id { get; set; }

    public int ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Country { get; set; }

    public string? Logo { get; set; }

    public string? Flag { get; set; }

    public ICollection<PlayerStatistics> Statistics { get; set; } = [];
}
