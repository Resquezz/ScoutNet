namespace ScoutNet.Domain.Entities;

public class ScoutReport
{
    public Guid Id { get; set; }

    public Guid ScoutId { get; set; }

    public Guid PlayerId { get; set; }

    public int CurrentForm { get; set; }

    public int Potential { get; set; }

    public string Pros { get; set; } = string.Empty;

    public string Cons { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public User Scout { get; set; } = null!;

    public Player Player { get; set; } = null!;
}
