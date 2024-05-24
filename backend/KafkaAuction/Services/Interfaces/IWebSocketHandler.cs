using System.Net.WebSockets;
using KafkaAuction.Enums;

namespace KafkaAuction.Services.Interfaces;

public interface IWebSocketHandler
{
    Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket, string auctionId, WebPages page);
}