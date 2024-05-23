using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Models;

public class Auction : Record
{
    [Key]
    public required string Auction_Id { get; set; }
    public required string Title { get; set; }
}
