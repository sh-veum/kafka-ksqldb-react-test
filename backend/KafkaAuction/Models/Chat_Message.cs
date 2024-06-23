using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public record Chat_Message
{
    [Key]
    public required string Message_Id { get; set; }
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public required string Message_Text { get; set; }
    // Because it wont create the table with an empty array for some reason
    public string[] Previous_Messages { get; set; } = ["initial"];
    public string Created_Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    public string[] Updated_Timestamps { get; set; } = ["initial"];
}
