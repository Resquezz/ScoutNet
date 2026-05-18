namespace ScoutNet.Domain.Entities;

public class Watchlist
{
    public Guid ScoutId { get; set; }

    public Guid PlayerId { get; set; }

    public User Scout { get; set; } = null!;

    public Player Player { get; set; } = null!;
}
