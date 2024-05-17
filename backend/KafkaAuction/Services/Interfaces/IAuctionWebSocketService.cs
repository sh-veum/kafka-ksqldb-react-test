using System.Net.WebSockets;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionWebSocketService
{
    Task SubscribeAsync(WebSocket webSocket, string auctionId);
}