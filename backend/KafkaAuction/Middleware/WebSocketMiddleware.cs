using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Middleware;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebSocketHandler _webSocketHandler;

    public WebSocketMiddleware(RequestDelegate next, IWebSocketHandler webSocketHandler)
    {
        _next = next;
        _webSocketHandler = webSocketHandler;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/ws"))
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var auctionId = context.Request.Query["auctionId"].ToString();
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await _webSocketHandler.HandleWebSocketAsync(context, webSocket, auctionId);
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
