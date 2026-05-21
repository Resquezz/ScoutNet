using Microsoft.EntityFrameworkCore;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Infrastructure.Persistence;

public class ScoutDbContext(DbContextOptions<ScoutDbContext> options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();

    public DbSet<PlayerStatistics> PlayerStatistics => Set<PlayerStatistics>();

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<League> Leagues => Set<League>();

    public DbSet<User> Users => Set<User>();

    public DbSet<ScoutReport> ScoutReports => Set<ScoutReport>();

    public DbSet<Watchlist> Watchlists => Set<Watchlist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(team => team.Id);
            entity.HasIndex(team => team.ExternalId).IsUnique();
            entity.Property(team => team.Name).HasMaxLength(200).IsRequired();
            entity.Property(team => team.Logo).HasMaxLength(500);
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.HasKey(league => league.Id);
            entity.HasIndex(league => league.ExternalId).IsUnique();
            entity.Property(league => league.Name).HasMaxLength(200).IsRequired();
            entity.Property(league => league.Country).HasMaxLength(100);
            entity.Property(league => league.Logo).HasMaxLength(500);
            entity.Property(league => league.Flag).HasMaxLength(500);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(player => player.Id);

            entity.HasIndex(player => player.ExternalId).IsUnique();

            entity.Property(player => player.Name).HasMaxLength(200).IsRequired();
            entity.Property(player => player.Firstname).HasMaxLength(100);
            entity.Property(player => player.Lastname).HasMaxLength(100);
            entity.Property(player => player.BirthPlace).HasMaxLength(200);
            entity.Property(player => player.BirthCountry).HasMaxLength(100);
            entity.Property(player => player.Nationality).HasMaxLength(100).IsRequired();
            entity.Property(player => player.Height).HasMaxLength(20);
            entity.Property(player => player.Weight).HasMaxLength(20);
            entity.Property(player => player.PhotoUrl).HasMaxLength(500);
            entity.Property(player => player.CurrentClub).HasMaxLength(200).IsRequired();

            entity.HasOne(player => player.TeamProfile)
                .WithMany()
                .HasForeignKey(player => player.TeamProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(player => player.LeagueProfile)
                .WithMany()
                .HasForeignKey(player => player.LeagueProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(player => player.Statistics)
                .WithOne(statistics => statistics.Player)
                .HasForeignKey(statistics => statistics.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(player => player.ScoutReports)
                .WithOne(report => report.Player)
                .HasForeignKey(report => report.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlayerStatistics>(entity =>
        {
            entity.HasKey(statistics => statistics.Id);

            entity.HasIndex(statistics => new { statistics.PlayerId, statistics.SeasonYear, statistics.LeagueId })
                .IsUnique();

            entity.Property(statistics => statistics.Position).HasMaxLength(50);
            entity.Property(statistics => statistics.Rating).HasPrecision(4, 2);

            entity.HasOne(statistics => statistics.Team)
                .WithMany(team => team.Statistics)
                .HasForeignKey(statistics => statistics.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(statistics => statistics.League)
                .WithMany(league => league.Statistics)
                .HasForeignKey(statistics => statistics.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);

            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasIndex(user => user.Username).IsUnique();

            entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<ScoutReport>(entity =>
        {
            entity.HasKey(report => report.Id);

            entity.Property(report => report.Pros).HasMaxLength(4000).IsRequired();
            entity.Property(report => report.Cons).HasMaxLength(4000).IsRequired();
            entity.Property(report => report.Summary).HasMaxLength(8000).IsRequired();

            entity.HasOne(report => report.Scout)
                .WithMany(user => user.ScoutReports)
                .HasForeignKey(report => report.ScoutId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.HasKey(watchlist => new { watchlist.ScoutId, watchlist.PlayerId });

            entity.HasOne(watchlist => watchlist.Scout)
                .WithMany(user => user.Watchlists)
                .HasForeignKey(watchlist => watchlist.ScoutId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(watchlist => watchlist.Player)
                .WithMany(player => player.Watchlists)
                .HasForeignKey(watchlist => watchlist.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
