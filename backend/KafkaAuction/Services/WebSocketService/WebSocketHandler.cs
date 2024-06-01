using System.Collections.Concurrent;
using System.Net.WebSockets;
using KafkaAuction.Enums;
using KafkaAuction.Services.Interfaces.WebSocketService;

namespace KafkaAuction.Services.WebSocketService;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly ILogger<WebSocketHandler> _logger;
    private readonly IAuctionWebSocketService _auctionWebSocketService;
    private readonly IChatWebSocketService _chatWebSocketService;
    private readonly ConcurrentDictionary<WebSocket, bool> _connections;

    public WebSocketHandler(ILogger<WebSocketHandler> logger, IAuctionWebSocketService auctionWebSocketService, IChatWebSocketService chatWebSocketService)
    {
        _logger = logger;
        _auctionWebSocketService = auctionWebSocketService;
        _chatWebSocketService = chatWebSocketService;
        _connections = new ConcurrentDictionary<WebSocket, bool>();
    }

    public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket, string auctionId, WebPages page)
    {
        _connections.TryAdd(webSocket, true);
        _logger.LogInformation("WebSocket connection established for {Page} with auctionId {AuctionId}.", page, auctionId);

        try
        {
            if (page == WebPages.AuctionOverview)
            {
                await _auctionWebSocketService.SubscribeToAuctionOverviewUpdatesAsync(webSocket);
            }
            else if (page == WebPages.SpesificAuction)
            {
                await _auctionWebSocketService.SubscribeToAuctionBidUpdatesAsync(webSocket, auctionId);
            }
            else if (page == WebPages.Chat)
            {
                await _chatWebSocketService.SubscribeToChatMessagesForAuctionAsync(webSocket, auctionId);
            }
            else
            {
                throw new InvalidOperationException("Invalid page");
            }

            // Keep the WebSocket open until closed by the client
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult? result;
            do
            {
                result = await ReceiveWebSocketMessageAsync(webSocket, buffer);
            } while (result != null && !result.CloseStatus.HasValue);
        }
        finally
        {
            _connections.TryRemove(webSocket, out _);
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            }
        }
    }

    private async Task<WebSocketReceiveResult?> ReceiveWebSocketMessageAsync(WebSocket webSocket, byte[] buffer)
    {
        if (webSocket.State != WebSocketState.Open)
            return null;

        try
        {
            return await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "Error receiving WebSocket message");
            return null;
        }
    }

    public int GetActiveConnectionsCount()
    {
        return _connections.Count;
    }
}
