using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public record User_Location
{
    [Key]
    public required string User_Location_Id { get; set; }
    public required string User_Id { get; set; }
    public required string[] Pages { get; set; }
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
