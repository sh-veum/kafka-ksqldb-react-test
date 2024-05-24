using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public class Auction : Record
{
    [Key]
    public required string Auction_Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required decimal Starting_Price { get; set; }
    public decimal? Current_Price { get; set; } = 0;
    public int? Number_Of_Bids { get; set; } = 0;
    public string? Winner { get; set; } = "No Winner Yet";
}
