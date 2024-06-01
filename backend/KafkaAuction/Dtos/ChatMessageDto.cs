namespace KafkaAuction.Dtos;

public class ChatMessageDto
{
    public required string Username { get; set; }
    public required string MessageText { get; set; }
    public string? Timestamp { get; set; }
}
