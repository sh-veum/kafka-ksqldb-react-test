using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IChatService _chatService;

    public ChatController(ILogger<ChatController> logger, IChatService chatService)
    {
        _logger = logger;
        _chatService = chatService;
    }

    [HttpPost("create_tables")]
    public async Task<IActionResult> CreateTables()
    {
        var results = await _chatService.CreateChatTableAsync();

        return Ok(results);
    }

    [HttpPost("insert_message")]
    public async Task<IActionResult> InsertMessage([FromBody] ChatMessageWithAuctionIdDto chatMessageDto)
    {
        var message = new Chat_Message
        {
            Message_Id = Guid.NewGuid().ToString(),
            Auction_Id = chatMessageDto.Auction_Id,
            Username = chatMessageDto.Username,
            MessageText = chatMessageDto.MessageText,
        };

        HttpResponseMessage result = await _chatService.InsertMessageAsync(message);

        if (!result.IsSuccessStatusCode)
        {
            return BadRequest(result.ReasonPhrase);
        }
        else
        {
            return Ok();
        }
    }

    [HttpPost("drop_tables")]
    public async Task<IActionResult> DropTables()
    {
        await _chatService.DropTablesAsync();

        return Ok();
    }

    [HttpGet("get_all_messages")]
    public async Task<IActionResult> GetAllMessages([FromQuery] bool sortByDate = false)
    {
        var messages = await _chatService.GetAllMessages();

        if (sortByDate)
        {
            messages = Sorter.SortByDate(messages, messages => messages.Timestamp!);
        }

        return Ok(messages);
    }

    [HttpGet("get_messages_for_auction")]
    [ProducesResponseType(typeof(ChatMessageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessagesForAuction([FromQuery] string auction_Id, [FromQuery] bool sortByDate = false)
    {
        var messages = await _chatService.GetMessagesForAuction(auction_Id);

        if (sortByDate)
        {
            messages = Sorter.SortByDate(messages, messages => messages.Timestamp!);
        }

        return Ok(messages);
    }

    [HttpGet("get_messages_for_auction_push_query")]
    public async Task<IActionResult> GetMessagesForAuctionPushQuery([FromQuery] string auction_Id)
    {
        var messages = await _chatService.GetMessagesForAuctionPushQuery(auction_Id);

        return Ok(messages);
    }
}