namespace KafkaAuction.Dtos;

public class AuctionCreatorDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required decimal Starting_Price { get; set; }
}