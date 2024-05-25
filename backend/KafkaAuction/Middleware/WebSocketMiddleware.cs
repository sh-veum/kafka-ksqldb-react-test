using KafkaAuction.Enums;
using KafkaAuction.Services.Interfaces.WebSocketService;

namespace KafkaAuction.Middleware;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly ILogger<WebSocketMiddleware> _logger;

    public WebSocketMiddleware(RequestDelegate next, IWebSocketHandler webSocketHandler, ILogger<WebSocketMiddleware> logger)
    {
        _next = next;
        _webSocketHandler = webSocketHandler;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/ws"))
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var auctionId = context.Request.Query["auctionId"].ToString();
                _logger.LogInformation("WebSocket connection requested for auctionId {AuctionId}.", auctionId);
                var webPage = Enum.Parse<WebPages>(context.Request.Query["webPage"].ToString());
                _logger.LogInformation("WebSocket connection requested for {Page}", webPage);
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await _webSocketHandler.HandleWebSocketAsync(context, webSocket, auctionId, webPage);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
        else
        {
            await _next(context);
        }
    }
}
