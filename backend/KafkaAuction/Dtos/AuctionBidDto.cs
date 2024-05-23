namespace KafkaAuction.Dtos;

public class AuctionBidDto
{
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
}