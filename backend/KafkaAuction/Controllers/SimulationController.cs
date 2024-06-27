using KafkaAuction.Initializers;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/simulation")]
public class SimulationController : ControllerBase
{
    private readonly IAuctionService _auctionService;
    private readonly IAuctionBidService _auctionBidService;
    private readonly IAuctionWithBidsService _auctionWithBidsService;
    private readonly IChatService _chatService;
    private readonly IUserLocationService _userLocationService;
    private readonly IKsqlDbService _ksqlDbService;
    private readonly ILogger<SimulationController> _logger;

    public SimulationController(
        IAuctionService auctionService,
        IAuctionBidService auctionBidService,
        IAuctionWithBidsService auctionWithBidsService,
        IChatService chatService,
        IUserLocationService userLocationService,
        IKsqlDbService ksqlDbService,
        ILogger<SimulationController> logger)
    {
        _auctionService = auctionService;
        _auctionBidService = auctionBidService;
        _auctionWithBidsService = auctionWithBidsService;
        _chatService = chatService;
        _userLocationService = userLocationService;
        _ksqlDbService = ksqlDbService;
        _logger = logger;
    }

    [HttpPost("inject_data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> StartSimulation([FromQuery] int? bidsPerAuction, [FromQuery] int? messagesPerAuction, [FromQuery] int? delay)
    {
        var config = new AuctionConfig
        {
            BidsPerAuction = bidsPerAuction ?? 8,
            MessagesPerAuction = messagesPerAuction ?? 5
        };

        var auctions = await _auctionService.GetAllAuctions();
        if (auctions.Count == 0)
        {
            _logger.LogInformation("No existing auctions found. Initializing new auctions.");
            await KsqlDbInitializer.InitializeAsync(_auctionService, _auctionBidService, _auctionWithBidsService, _chatService, _userLocationService, _ksqlDbService, _logger, config);
        }
        else
        {
            _logger.LogInformation("Existing auctions found. Adding bids and messages to them.");
            foreach (var auction in auctions)
            {
                await InsertBidsForAuction(auction.Auction_Id, auction.Current_Price ?? 0, config.BidsPerAuction, delay ?? 1 * 100);
                await InsertMessagesForAuction(auction.Auction_Id, config.MessagesPerAuction, delay ?? 1 * 100);
            }
        }

        return Ok("Simulation started and data inserted.");
    }

    private async Task InsertBidsForAuction(string auctionId, decimal currentPrice, int bidCount, int delay)
    {
        var bidderNames = BidderNames.GetBidderNames();
        var random = new Random();
        for (int i = 1; i <= bidCount; i++)
        {
            var bidAmount = currentPrice + (i * 500); // Ensure the next bid is higher than the current bid
            var auctionBid = new Auction_Bid
            {
                Bid_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = bidderNames[random.Next(bidderNames.Length)],
                Bid_Amount = bidAmount,
                Timestamp = DateTime.UtcNow.AddSeconds(i).ToString("yyyy-MM-dd HH:mm:ss")
            };
            await _auctionBidService.InsertBidAsync(auctionBid);
            currentPrice = bidAmount; // Update current price for the next bid
            await Task.Delay(delay);
        }
    }

    private async Task InsertMessagesForAuction(string auctionId, int messageCount, int delay)
    {
        var characterSentences = CharacterSentences.GetCharacterSentences();
        var random = new Random();
        for (int i = 1; i <= messageCount; i++)
        {
            var username = characterSentences.Keys.ElementAt(random.Next(characterSentences.Count));
            var messageContents = characterSentences[username];
            var chatMessage = new Chat_Message
            {
                Message_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = username,
                Message_Text = messageContents[random.Next(messageContents.Length)],
                Created_Timestamp = DateTime.UtcNow.AddSeconds(i).ToString("yyyy-MM-dd HH:mm:ss"),
            };
            await _chatService.InsertMessageAsync(chatMessage);
            await Task.Delay(delay);
        }
    }
}
