using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public record Auction
{
    [Key]
    public required string Auction_Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required decimal Starting_Price { get; set; }
    public decimal? Current_Price { get; set; } = 0;
    public int? Number_Of_Bids { get; set; } = 0;
    public string? Winner { get; set; }
    public string Created_At { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
