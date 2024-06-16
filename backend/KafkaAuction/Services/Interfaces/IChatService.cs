using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IChatService
{
    Task<TablesResponse[]> CreateChatTableAsync(CancellationToken cancellationToken = default);
    Task<List<DropResourceResponseDto>> DropTablesAsync();
    Task<(HttpResponseMessage httpResponseMessage, ChatMessageWithAuctionIdDto chatMessageDto)> InsertMessageAsync(Chat_Message message);
    Task<List<ChatMessageWithAuctionIdDto>> GetAllMessages();
    Task<List<ChatMessageDto>> GetMessagesForAuction(string auction_id);
    Task<List<ChatMessageDto>> GetMessagesForAuctionPushQuery(string auction_id);
}
