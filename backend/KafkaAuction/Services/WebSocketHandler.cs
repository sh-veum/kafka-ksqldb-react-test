using System.Net.WebSockets;
using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Services;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly IAuctionWebSocketService _auctionWebSocketService;

    public WebSocketHandler(IAuctionWebSocketService auctionWebSocketService)
    {
        _auctionWebSocketService = auctionWebSocketService;
    }

    public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket, string auctionId)
    {
        await _auctionWebSocketService.SubscribeAsync(webSocket, auctionId);
    }
}