using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public record Chat_Message
{
    [Key]
    public required string Message_Id { get; set; }
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public required string MessageText { get; set; }
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
