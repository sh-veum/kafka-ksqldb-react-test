using System.Collections.Concurrent;
using System.Net;
using System.Reactive.Linq;
using KafkaAuction.Constants;
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
    private readonly string _chatMessageTableName = TableNameConstants.ChatMessages;

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

    public async Task<(HttpResponseMessage httpResponseMessage, ChatMessageDetailedDto chatMessageDto)> InsertMessageAsync(Chat_Message message)
    {
        var inserter = new EntityInserter<Chat_Message>(_restApiProvider, _logger);
        var response = await inserter.InsertAsync(_chatMessageTableName, message);

        var chatMessageDto = new ChatMessageDetailedDto
        {
            Message_Id = message.Message_Id,
            Auction_Id = message.Auction_Id,
            Username = message.Username,
            Message_Text = message.Message_Text,
            Previous_Messages = message.Previous_Messages ?? [],
            Created_Timestamp = message.Created_Timestamp,
            Updated_Timestamps = message.Updated_Timestamps ?? []
        };

        return (response, chatMessageDto);
    }

    public async Task<Chat_Message?> GetMessageById(string message_id)
    {
        var chatMessage = await _context.CreatePullQuery<Chat_Message>($"queryable_{_chatMessageTableName}")
            .Where(p => p.Message_Id == message_id)
            .FirstOrDefaultAsync();

        if (chatMessage == null)
        {
            _logger.LogWarning($"Message with id {message_id} not found");
            return null;
        }

        return chatMessage;
    }

    public async Task<(HttpResponseMessage httpResponseMessage, ChatMessageDetailedDto? chatMessageDto)> UpdateMessageAsync(ChatMessageUpdateDto chatMessageUpdateDto)
    {
        var message = await GetMessageById(chatMessageUpdateDto.Message_Id);

        if (message == null)
        {
            return (new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Message not found")
            }, null);
        }

        message.Previous_Messages ??= [];
        message.Updated_Timestamps ??= [];

        message.Previous_Messages = message.Previous_Messages?.Append(message.Message_Text).ToArray() ?? [message.Message_Text];
        message.Updated_Timestamps = message.Updated_Timestamps?.Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")).ToArray() ?? [DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")];

        message.Message_Text = chatMessageUpdateDto.Message_Text;

        var (httpResponseMessage, chatMessage) = await InsertMessageAsync(message);

        return (httpResponseMessage, chatMessage);
    }

    public async Task<List<DropResourceResponseDto>> DropTablesAsync()
    {
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        var resourcesToDrop = new List<(string Name, ResourceType Type)>
        {
            ("QUERYABLE_" + _chatMessageTableName, ResourceType.Table),
            (_chatMessageTableName, ResourceType.Table)
        };

        var responseList = new List<DropResourceResponseDto>();

        foreach (var resource in resourcesToDrop)
        {
            var response = await dropper.DropResourceAsync(resource.Name, resource.Type);
            responseList.Add(new DropResourceResponseDto
            {
                ResourceName = resource.Name,
                IsSuccess = response.IsSuccessStatusCode
            });
        }

        return responseList;
    }

    public async Task<List<ChatMessageDetailedDto>> GetAllMessages()
    {
        var chatMessages = _context.CreatePullQuery<Chat_Message>($"queryable_{_chatMessageTableName}")
            .GetManyAsync();

        List<ChatMessageDetailedDto> chatMessageDtos = [];

        await foreach (var chatMessage in chatMessages.ConfigureAwait(false))
        {
            chatMessageDtos.Add(new ChatMessageDetailedDto
            {
                Message_Id = chatMessage.Message_Id,
                Auction_Id = chatMessage.Auction_Id,
                Username = chatMessage.Username,
                Message_Text = chatMessage.Message_Text,
                Previous_Messages = chatMessage.Previous_Messages ?? [],
                Created_Timestamp = chatMessage.Created_Timestamp,
                Updated_Timestamps = chatMessage.Updated_Timestamps ?? []
            });
        }

        return chatMessageDtos;
    }

    /// <summary>
    /// Gets messages for an auction using a pull query<br />
    /// <b>Pro</b>: Fast-ish<br />
    /// <b>Con</b>: Response is not sorted
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
            var hasBeenEdited = chatMessage.Updated_Timestamps?.Length > 1;

            chatMessageDtos.Add(new ChatMessageDto
            {
                Message_Id = chatMessage.Message_Id,
                Username = chatMessage.Username,
                Message_Text = chatMessage.Message_Text,
                Created_Timestamp = chatMessage.Created_Timestamp,
                Is_Edited = hasBeenEdited
            });
        }

        return chatMessageDtos;
    }

    /// <summary>
    /// Get messages for an auction using a push query<br />
    /// <b>Pro</b>: Response is already sorted<br />
    /// <b>Con</b>: Slow as hell compared to pull queries
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
                        var hasBeenEdited = message.Updated_Timestamps?.Length > 1;

                        messages.Add(new ChatMessageDto
                        {
                            Message_Id = message.Message_Id,
                            Username = message.Username,
                            Message_Text = message.Message_Text,
                            Created_Timestamp = message.Created_Timestamp,
                            Is_Edited = hasBeenEdited
                        });
                        _logger.LogInformation($"Received message: {message.Message_Text}");
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