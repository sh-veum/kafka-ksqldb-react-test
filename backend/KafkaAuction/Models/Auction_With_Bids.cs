using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public record Auction_With_Bids
{
    [Key]
    public required string Auction_With_Bids_Id { get; set; }
    public required string Auction_Id { get; set; }
    public required string Auction_Bid_Id { get; set; }
}