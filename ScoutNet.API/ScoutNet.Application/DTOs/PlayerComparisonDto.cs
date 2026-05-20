namespace ScoutNet.Application.DTOs;

public class PlayerComparisonDto
{
    public IReadOnlyList<Entry> Players { get; set; } = [];

    public class Entry
    {
        public PlayerDto Profile { get; set; } = null!;

        public IReadOnlyList<PlayerStatisticsDto> Statistics { get; set; } = [];
    }
}
