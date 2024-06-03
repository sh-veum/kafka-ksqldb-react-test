using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public record Auction_With_Bids
{
    [Key]
    public required string Auction_Id { get; set; }
    public required string Title { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public required string Timestamp { get; set; }
}