namespace KafkaAuction.Dtos;

public class AuctionBidCreatorDto
{
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
}