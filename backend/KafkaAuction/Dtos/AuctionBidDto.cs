namespace KafkaAuction.Dtos;

public class AuctionBidDto
{
    public int AuctionId { get; set; }
    public required string Username { get; set; }
    public decimal BidAmount { get; set; }
}