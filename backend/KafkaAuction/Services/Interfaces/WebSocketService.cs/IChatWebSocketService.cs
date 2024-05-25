using System.Net.WebSockets;

namespace KafkaAuction.Services.Interfaces.WebSocketService;

public interface IChatWebSocketService
{
    Task SubscribeToChatMessagesForAuctionAsync(WebSocket webSocket, string auctionId);
}