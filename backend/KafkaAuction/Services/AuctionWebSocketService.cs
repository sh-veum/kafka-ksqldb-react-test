using System.Net.WebSockets;
using System.Text;
using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.Linq;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using Newtonsoft.Json;

namespace KafkaAuction.Services;

public class AuctionWebSocketService : IAuctionWebSocketService
{
    private readonly ILogger<AuctionWebSocketService> _logger;
    private readonly IConfiguration _configuration;

    public AuctionWebSocketService(ILogger<AuctionWebSocketService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SubscribeAsync(WebSocket webSocket, string auctionId)
    {
        _logger.LogInformation($"Subscribing to WebSocket for auctionId: {auctionId}");
        var ksqlDbUrl = _configuration.GetValue<string>("KSqlDb:Url");
        _logger.LogInformation($"KSqlDb:Url: {ksqlDbUrl}");

        if (string.IsNullOrWhiteSpace(ksqlDbUrl))
        {
            throw new InvalidOperationException("KSqlDb:Url configuration is missing");
        }

        var contextOptions = new KSqlDBContextOptions(ksqlDbUrl)
        {
            ShouldPluralizeFromItemName = true
        };

        using var context = new KSqlDBContext(contextOptions);

        var auctionIdInt = int.Parse(auctionId);

        var subscription = context.CreatePushQuery<Auction_Bid>()
            .WithOffsetResetPolicy(AutoOffsetReset.Earliest)
            .Where(p => p.Auction_Id == auctionIdInt)
            .Select(l => new AuctionBidDto
            {
                Auction_Id = l.Auction_Id,
                Username = l.Username,
                Bid_Amount = l.Bid_Amount,
            })
            .Subscribe(AuctionBidDto =>
            {
                var message = JsonConvert.SerializeObject(AuctionBidDto);
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
