namespace PaceFriendsBackend.Data;

using Microsoft.EntityFrameworkCore;

public class PaceFriendsDbContext : DbContext
{
    public PaceFriendsDbContext(DbContextOptions<PaceFriendsDbContext> options) : base(options) { }

    public DbSet<Player> Players { get; set; }
    public DbSet<Day> Days { get; set; }
    public DbSet<RoutePoint> RoutePoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .HasMany(p => p.Days)
            .WithOne(d => d.Player)
            .HasForeignKey(d => d.PlayerID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Day>()
            .HasMany(d => d.RoutePoints)
            .WithOne(r => r.Day)
            .HasForeignKey(r => r.DayID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<RoutePoint>()
            .HasIndex(p => new { p.DayID, p.SequenceOrder }); 

    }
}
