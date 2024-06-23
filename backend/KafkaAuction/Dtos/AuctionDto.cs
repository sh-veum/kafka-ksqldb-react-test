namespace KafkaAuction.Dtos;

public class AuctionDto
{
    public required string Auction_Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal Starting_Price { get; set; }
    public decimal? Current_Price { get; set; }
    public required string Leader { get; set; }
    public int? Number_Of_Bids { get; set; } = 0;
    public string? Winner { get; set; }
    public required string Created_At { get; set; }
    public required string End_Date { get; set; }
    public bool Is_Open { get; set; }
}