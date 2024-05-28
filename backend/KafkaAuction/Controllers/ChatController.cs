using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
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
    public async Task<IActionResult> GetAllMessages()
    {
        var messages = await _chatService.GetAllMessages();

        return Ok(messages);
    }

    [HttpGet("get_messages_for_auction")]
    public async Task<IActionResult> GetMessagesForAuction([FromQuery] string auction_Id)
    {
        var messages = await _chatService.GetMessagesForAuction(auction_Id);

        return Ok(messages);
    }

    [HttpGet("get_messages_for_auction_alternative")]
    public async Task<IActionResult> GetMessagesForAuctionAlt([FromQuery] string auction_Id)
    {
        var messages = await _chatService.GetMessagesForAuctionAlternative(auction_Id);

        return Ok(messages);
    }
}