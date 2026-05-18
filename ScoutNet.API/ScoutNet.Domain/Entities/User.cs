using ScoutNet.Domain.Enums;

namespace ScoutNet.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public ICollection<Watchlist> Watchlists { get; set; } = [];

    public ICollection<ScoutReport> ScoutReports { get; set; } = [];
}
