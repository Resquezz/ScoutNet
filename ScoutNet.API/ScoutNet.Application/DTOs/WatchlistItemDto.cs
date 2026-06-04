namespace ScoutNet.Application.DTOs;

public class WatchlistItemDto
{
    public int PlayerId { get; set; }

    public PlayerDto Player { get; set; } = new();
}
