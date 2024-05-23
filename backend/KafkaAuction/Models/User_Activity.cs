using System.ComponentModel.DataAnnotations;
using ksqlDB.RestApi.Client.KSql.Query;

namespace KafkaAuction.Models;

public class User_Activity : Record
{
    [Key]
    public required string User_Id { get; set; }
    public required string Activity_Type { get; set; }
    public DateTime Timestamp { get; set; }
}
