using System.Net.WebSockets;

namespace KafkaAuction.Services.Interfaces;

public interface IWebSocketHandler
{
    Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket, string auctionId);
}