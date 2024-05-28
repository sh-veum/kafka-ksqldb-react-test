using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IChatService
{
    Task<TablesResponse[]> CreateChatTableAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> InsertMessageAsync(Chat_Message message);
    Task DropTablesAsync();
    Task<List<ChatMessageWithAuctionIdDto>> GetAllMessages();
    Task<List<ChatMessageDto>> GetMessagesForAuction(string auction_id);
    Task<List<ChatMessageDto>> GetMessagesForAuctionAlternative(string auction_id);
}
