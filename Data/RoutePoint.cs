namespace PaceFriendsBackend.Data;

using System.ComponentModel.DataAnnotations;

public class RoutePoint
{
    [Key]
    public Guid PointID { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int SequenceOrder { get; set; }

    // Foreign Key to Day
    public Guid DayID { get; set; }
    public Day Day { get; set; } = null!;
}
