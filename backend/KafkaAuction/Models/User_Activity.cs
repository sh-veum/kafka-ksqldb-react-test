using System.ComponentModel.DataAnnotations;

namespace KafkaAuction.Models;

public record User_Activity
{
    [Key]
    public required string User_Id { get; set; }
    public required string Activity_Type { get; set; }
    public DateTime Timestamp { get; set; }
}
