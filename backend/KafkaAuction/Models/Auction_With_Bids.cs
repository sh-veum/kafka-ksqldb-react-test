using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

class Auction_With_Bids : Record
{
    [Key]
    public required string Auction_With_Bids_Id { get; set; }
    public required string Auction_Id { get; set; }
    public required string Auction_Bid_Id { get; set; }
}