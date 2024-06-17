namespace KafkaAuction.Dtos;

public class ChatMessageDetailedDto
{
    public required string Auction_Id { get; set; }
    public required string Message_Id { get; set; }
    public required string Username { get; set; }
    public required string Message_Text { get; set; }
    public required string[] Previous_Messages { get; set; }
    public required string Created_Timestamp { get; set; }
    public required string[] Updated_Timestamps { get; set; }
}
