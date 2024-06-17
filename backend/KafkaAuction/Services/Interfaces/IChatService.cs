using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IChatService
{
    Task<TablesResponse[]> CreateChatTableAsync(CancellationToken cancellationToken = default);
    Task<List<DropResourceResponseDto>> DropTablesAsync();
    Task<(HttpResponseMessage httpResponseMessage, ChatMessageDetailedDto chatMessageDto)> InsertMessageAsync(Chat_Message message);
    Task<Chat_Message?> GetMessageById(string message_id);
    Task<(HttpResponseMessage httpResponseMessage, ChatMessageDetailedDto? chatMessageDto)> UpdateMessageAsync(ChatMessageUpdateDto chatMessageUpdateDto);
    Task<List<ChatMessageDetailedDto>> GetAllMessages();
    Task<List<ChatMessageDto>> GetMessagesForAuction(string auction_id);
    Task<List<ChatMessageDto>> GetMessagesForAuctionPushQuery(string auction_id);
}
