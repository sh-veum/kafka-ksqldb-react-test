using KafkaAuction.Services.Interfaces.WebSocketService;

namespace KafkaAuction.Services.WebSocketService;

public class WebSocketLoggerService : BackgroundService
{
    private readonly ILogger<WebSocketLoggerService> _logger;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public WebSocketLoggerService(ILogger<WebSocketLoggerService> logger, IWebSocketHandler webSocketHandler)
    {
        _logger = logger;
        _webSocketHandler = webSocketHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Number of active WebSocket connections: {_webSocketHandler.GetActiveConnectionsCount()}");
            await Task.Delay(_interval, stoppingToken);
        }
    }
}