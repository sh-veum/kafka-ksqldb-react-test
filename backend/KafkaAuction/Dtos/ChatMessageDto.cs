namespace KafkaAuction.Dtos;

public class ChatMessageDto
{
    public required string Message_Id { get; set; }
    public required string Username { get; set; }
    public required string Message_Text { get; set; }
    public required string Created_Timestamp { get; set; }
    public required bool Is_Edited { get; set; }
}
