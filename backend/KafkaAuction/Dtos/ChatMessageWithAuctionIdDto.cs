namespace KafkaAuction.Dtos;

public class ChatMessageWithAuctionIdDto : ChatMessageDto
{
    public required string Auction_Id { get; set; }
}