namespace KafkaAuction.Dtos;

public class AuctionWithBidsDto
{
    public int Auction_Id { get; set; }
    public required string Title { get; set; }
    public required string Username { get; set; }
    public decimal Bid_Amount { get; set; }
    public DateTime Bid_Time { get; set; }
}