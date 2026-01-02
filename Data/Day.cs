namespace PaceFriendsBackend.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Day
{
    [Key]
    public Guid DayID { get; set; }

    public DateTime Date { get; set; }
    public long TotalSteps { get; set; }
    public long TotalCalories { get; set; }
    public long TimeSpent { get; set; }

    // FK Player
    public Guid PlayerID { get; set; }
    public Player Player { get; set; } = null!;

    // Navigation Property: One Day has many RoutePoints (The list of coords)
    public List<RoutePoint> RoutePoints { get; set; } = new();
}
