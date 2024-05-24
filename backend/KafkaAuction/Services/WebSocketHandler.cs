using System.Net.WebSockets;
using KafkaAuction.Enums;
using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Services;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly IAuctionWebSocketService _auctionWebSocketService;
    private readonly ILogger<WebSocketHandler> _logger;

    public WebSocketHandler(IAuctionWebSocketService auctionWebSocketService, ILogger<WebSocketHandler> logger)
    {
        _auctionWebSocketService = auctionWebSocketService;
        _logger = logger;
    }

    public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket, string auctionId, WebPages page)
    {
        _logger.LogInformation("WebSocket connection established for {Page} with auctionId {AuctionId}.", page, auctionId);

        if (page == WebPages.AuctionOverview)
        {
            await _auctionWebSocketService.SubscribeToAuctionOverviewUpdatesAsync(webSocket);
        }
        else if (page == WebPages.SpesificAuction)
        {

            await _auctionWebSocketService.SubscribeToAuctionBidUpdatesAsync(webSocket, auctionId);
        }
        else
        {
            throw new InvalidOperationException("Invalid page");
        }
    }
}