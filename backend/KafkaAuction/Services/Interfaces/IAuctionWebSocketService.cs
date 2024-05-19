using System.Net.WebSockets;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionWebSocketService
{
    Task SubscribeToAuctionUpdatesAsync(WebSocket webSocket, string auctionId);
}