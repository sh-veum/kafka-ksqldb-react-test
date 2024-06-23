namespace KafkaAuction.Models;

public record Auction_With_Bids
{
    public required string Auction_With_Bids_Id { get; set; }
    public required string Bid_Id { get; set; }
    public required string Title { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public required string Timestamp { get; set; }
}