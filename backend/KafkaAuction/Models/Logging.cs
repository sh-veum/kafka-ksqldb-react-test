namespace KafkaAuction.Models;

public class Logging
{
    public int Id { get; set; }
    public required string Message { get; set; }

    public DateTime TimeStamp { get; set; }
}
