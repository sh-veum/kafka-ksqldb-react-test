using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public class Chat_Message : Record
{
    [Key]
    public required string Message_Id { get; set; }
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public required string MessageText { get; set; }
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
