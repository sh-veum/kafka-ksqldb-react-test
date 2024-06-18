namespace KafkaAuction.Dtos;

public class AuctionBidDto : AuctionBidCreatorDto
{
    public required string Timestamp { get; set; }
}