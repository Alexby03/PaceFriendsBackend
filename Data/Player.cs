using Microsoft.EntityFrameworkCore;

namespace PaceFriendsBackend.Data;

using System.ComponentModel.DataAnnotations;

[Index(nameof(Email), IsUnique = true)] 
public class Player
{
    [Key]
    public Guid PlayerID { get; set; } // Maps to CHAR(36) automatically in MySQL

    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } 
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string Gender { get; set; } = string.Empty;

    // Stats
    public int CurrentStreak { get; set; }
    public bool CompletedDaily { get; set; }
    public long WeekScore { get; set; } // Used for the leaderboard ranking
    public long TotalTimePlayed { get; set; }
    public long WeeklySteps { get; set; }
    public DateTime LastUpdated { get; set; }
    public long TotalScore { get; set; } 

    // Navigation Property: One Player has many Days
    public List<Day> Days { get; set; } = new();
}
