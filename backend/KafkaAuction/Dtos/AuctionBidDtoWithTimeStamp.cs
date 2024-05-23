namespace KafkaAuction.Dtos;

public class AuctionBidDtoWithTimeStamp : AuctionBidDto
{
    public required string Timestamp { get; set; }
}