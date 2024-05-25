using System.Net.WebSockets;
using System.Text;
using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces.WebSocketService;
using ksqlDB.RestApi.Client.KSql.Linq;
using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using Newtonsoft.Json;

namespace KafkaAuction.Services.WebSocketService;

public class ChatWebSocketService : IChatWebSocketService
{
    private readonly ILogger<ChatWebSocketService> _logger;
    private readonly KSqlDBContext _context;

    public ChatWebSocketService(ILogger<ChatWebSocketService> logger, KSqlDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task SubscribeToChatMessagesForAuctionAsync(WebSocket webSocket, string auctionId)
    {
        _logger.LogInformation($"Subscribing to Chat Messages for auctionId: {auctionId}");

        var subscription = _context.CreatePushQuery<Chat_Message>()
            .WithOffsetResetPolicy(AutoOffsetReset.Earliest)
            .Where(p => p.Auction_Id == auctionId)
            .Select(l => new ChatMessageDto
            {
                Username = l.Username,
                MessageText = l.MessageText,
                Timestamp = l.Timestamp
            })
            .Subscribe(ChatMessageDto =>
            {
                var message = JsonConvert.SerializeObject(ChatMessageDto);
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);

                _logger.LogInformation($"Sending message: {message}");

                webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            },
            error => _logger.LogError(error, "Error in WebSocket subscription"));

        _logger.LogInformation("WebSocket subscription completed");

        // Keep the WebSocket open until closed by the client
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;
        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}
