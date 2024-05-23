using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public class Auction_Bid : Record
{
    [Key]
    public required string Bid_Id { get; set; }
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public required string Timestamp { get; set; } // DateTime
}