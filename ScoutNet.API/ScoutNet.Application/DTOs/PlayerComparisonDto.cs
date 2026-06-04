namespace ScoutNet.Application.DTOs;

public class PlayerComparisonDto
{
    public int SeasonYear { get; set; }

    public IReadOnlyList<Entry> Players { get; set; } = [];

    public PlayerStatisticsDeltaDto MetricsDelta { get; set; } = new();

    public class Entry
    {
        public PlayerDto Profile { get; set; } = null!;

        public PlayerStatisticsDto? SeasonStatistics { get; set; }

        public PlayerSkillsDto Skills { get; set; } = new();
    }
}
