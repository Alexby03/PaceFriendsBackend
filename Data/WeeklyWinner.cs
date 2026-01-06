namespace PaceFriendsBackend.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("WeeklyWinners")]
public class WeeklyWinner
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; } = 1;

    public Guid PlayerID { get; set; }
    
    public string FullName { get; set; } = string.Empty;
    
    public long Score { get; set; }
}
