namespace ScoutNet.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }

    public int ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Logo { get; set; }

    public ICollection<PlayerStatistics> Statistics { get; set; } = [];
}
