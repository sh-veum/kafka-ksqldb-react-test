namespace KafkaAuction.Dtos;

public class AuctionWithBidDto
{
    public string? Auction_Id { get; set; }
    public string? Bid_Id { get; set; }
    public required string Title { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public required string Timestamp { get; set; }
}
