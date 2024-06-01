using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Text;
using KafkaAuction.Dtos;
using KafkaAuction.Enums;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Linq;
using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;


namespace KafkaAuction.Services;

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly KSqlDBContext _context;
    private readonly string _chatMessageTableName = "CHAT_MESSAGES";

    public ChatService(ILogger<ChatService> logger, IKSqlDbRestApiProvider restApiProvider, IConfiguration configuration)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;

        var _ksqlDbUrl = configuration.GetValue<string>("KSqlDb:Url") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_ksqlDbUrl))
        {
            throw new InvalidOperationException("KSqlDb:Url configuration is missing");
        }

        var contextOptions = new KSqlDBContextOptions(_ksqlDbUrl)
        {
            ShouldPluralizeFromItemName = true
        };

        _context = new KSqlDBContext(contextOptions);
    }

    public async Task<TablesResponse[]> CreateChatTableAsync(CancellationToken cancellationToken = default)
    {
        var chatMessageTableCreator = new TableCreator<Chat_Message>(_restApiProvider, _logger);
        if (!await chatMessageTableCreator.CreateTableAsync(_chatMessageTableName, cancellationToken))
        {
            throw new InvalidOperationException("Failed to create table");

        }

        await chatMessageTableCreator.CreateQueryableTableAsync(_chatMessageTableName, cancellationToken);

        return await _restApiProvider.GetTablesAsync(cancellationToken);
    }

    public async Task<HttpResponseMessage> InsertMessageAsync(Chat_Message message)
    {
        var inserter = new EntityInserter<Chat_Message>(_restApiProvider, _logger);
        return await inserter.InsertAsync(_chatMessageTableName, message);
    }

    public async Task DropTablesAsync()
    {
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        await dropper.DropResourceAsync("QUERYABLE_" + _chatMessageTableName, ResourceType.Table);
        await dropper.DropResourceAsync(_chatMessageTableName, ResourceType.Table);
    }

    public async Task<List<ChatMessageWithAuctionIdDto>> GetAllMessages()
    {
        var chatMessages = _context.CreatePullQuery<Chat_Message>($"queryable_{_chatMessageTableName}")
            .GetManyAsync();

        List<ChatMessageWithAuctionIdDto> chatMessageDtos = [];

        await foreach (var chatMessage in chatMessages.ConfigureAwait(false))
        {
            chatMessageDtos.Add(new ChatMessageWithAuctionIdDto
            {
                Auction_Id = chatMessage.Auction_Id,
                Username = chatMessage.Username,
                MessageText = chatMessage.MessageText,
                Timestamp = chatMessage.Timestamp
            });
        }

        return chatMessageDtos;
    }

    /// <summary>
    /// Gets messages for an auction using a pull query<br />
    /// Pro: Fast-ish<br />
    /// Con: Response is not sorted
    /// </summary>
    /// <param name="auction_id">Id of auction to get messages from</param>
    /// <returns>A list of chat messages</returns>
    public async Task<List<ChatMessageDto>> GetMessagesForAuction(string auction_id)
    {
        var chatMessages = _context.CreatePullQuery<Chat_Message>($"queryable_{_chatMessageTableName}")
            .Where(p => p.Auction_Id == auction_id)
            .GetManyAsync();

        List<ChatMessageDto> chatMessageDtos = [];

        await foreach (var chatMessage in chatMessages.ConfigureAwait(false))
        {
            chatMessageDtos.Add(new ChatMessageDto
            {
                Username = chatMessage.Username,
                MessageText = chatMessage.MessageText,
                Timestamp = chatMessage.Timestamp
            });
        }

        return chatMessageDtos;
    }

    /// <summary>
    /// Get messages for an auction using a push query<br />
    /// Pro: Response is already sorted<br />
    /// Con: Slow as hell compared to pull queries
    /// </summary>
    /// <param name="auction_id">Id of auction to get messages from</param>
    /// <returns>A list of chat messages</returns>
    public async Task<List<ChatMessageDto>> GetMessagesForAuctionPushQuery(string auction_id)
    {
        var messages = new ConcurrentBag<ChatMessageDto>();
        var tcs = new TaskCompletionSource<List<ChatMessageDto>>();

        var queryStream = _context.CreatePushQuery<Chat_Message>()
            .Where(m => m.Auction_Id == auction_id)
            .ToObservable();

        using var subscription = queryStream
            // Buffer messages for 3 seconds or 100 messages
            .Buffer(TimeSpan.FromSeconds(3), 100)
            .Where(buffer => buffer.Count > 0)
            .Subscribe(
                buffer =>
                {
                    foreach (var message in buffer)
                    {
                        messages.Add(new ChatMessageDto
                        {
                            Username = message.Username,
                            MessageText = message.MessageText,
                            Timestamp = message.Timestamp
                        });
                        _logger.LogInformation($"Received message: {message.MessageText}");
                    }
                    tcs.TrySetResult([.. messages]);
                },
                onError: error =>
                {
                    _logger.LogError(error, "Error while processing push query");
                    tcs.TrySetException(error);
                });

        // Await for the buffer to fill or an error to occur
        return await tcs.Task;
    }
}