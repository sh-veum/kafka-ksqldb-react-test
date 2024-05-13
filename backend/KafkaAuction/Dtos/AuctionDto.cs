namespace KafkaAuction.Dtos;

public class AuctionDto
{
    public int? AuctionId { get; set; }
    public required string Title { get; set; }
}