namespace KafkaAuction.Dtos;

public class AuctionBidMessageDto
{
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public required string Timestamp { get; set; }
}