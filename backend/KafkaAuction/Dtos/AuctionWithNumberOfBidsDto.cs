

namespace KafkaAuction.Dtos;

public class AuctionWithNumberOfBidsDto : AuctionDto
{
    public required int NumberOfBids { get; set; }
}