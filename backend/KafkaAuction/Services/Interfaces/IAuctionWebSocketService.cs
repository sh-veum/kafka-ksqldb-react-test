using System.Net.WebSockets;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionWebSocketService
{
    Task SubscribeToAuctionBidUpdatesAsync(WebSocket webSocket, string auctionId);
    Task SubscribeToAuctionOverviewUpdatesAsync(WebSocket webSocket);
}