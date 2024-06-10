using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Tests.Model;

public class Test
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
}
