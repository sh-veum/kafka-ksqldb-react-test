using System.Net.WebSockets;
using KafkaAuction.Enums;
using KafkaAuction.Services.Interfaces.WebSocketService;

namespace KafkaAuction.Services.WebSocketService;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly ILogger<WebSocketHandler> _logger;
    private readonly IAuctionWebSocketService _auctionWebSocketService;
    private readonly IChatWebSocketService _chatWebSocketService;

    public WebSocketHandler(ILogger<WebSocketHandler> logger, IAuctionWebSocketService auctionWebSocketService, IChatWebSocketService chatWebSocketService)
    {
        _logger = logger;
        _auctionWebSocketService = auctionWebSocketService;
        _chatWebSocketService = chatWebSocketService;
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
        else if (page == WebPages.Chat)
        {
            await _chatWebSocketService.SubscribeToChatMessagesForAuctionAsync(webSocket, auctionId);
        }
        else
        {
            throw new InvalidOperationException("Invalid page");
        }
    }
}