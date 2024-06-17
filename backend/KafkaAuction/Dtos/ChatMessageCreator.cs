namespace KafkaAuction.Dtos;

public class ChatMessageCreatorDto
{
    public required string Auction_Id { get; set; }
    public required string Username { get; set; }
    public required string Message_Text { get; set; }
}
