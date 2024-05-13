using System.ComponentModel.DataAnnotations;
using ksqlDB.RestApi.Client.KSql.Query;

namespace KafkaAuction.Models;

public class Auction_Bid : Record
{
    [Key]
    public int Id { get; set; }
    public int AuctionId { get; set; }
    public required string Username { get; set; }
    public decimal BidAmount { get; set; }
}