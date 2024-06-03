namespace KafkaAuction.Dtos;

public class AuctionWithBidDto
{
    public required string Title { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public required string Timestamp { get; set; }
}
