using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace KafkaAuction.Controllers;

[ApiController]
[Route("api/auction")]
public class AuctionController : ControllerBase
{
    private readonly ILogger<AuctionController> _logger;
    private readonly IAuctionService _auctionService;

    public AuctionController(ILogger<AuctionController> logger, IAuctionService auctionService)
    {
        _logger = logger;
        _auctionService = auctionService;
    }

    [HttpPost("create_tables")]
    public async Task<IActionResult> CreateTables()
    {
        var results = await _auctionService.CreateAuctionTableAsync();

        return Ok(results);
    }

    [HttpPost("create_streams")]
    public async Task<IActionResult> CreateStreams()
    {
        var results = await _auctionService.CreateAuctionBidStreamAsync();

        return Ok(results);
    }

    [HttpPost("crate_auction_with_bids_stream")]
    public async Task<IActionResult> CreateAuctionWithBidsStream()
    {
        var results = await _auctionService.CreateAuctionsWithBidsStreamAsync();

        return Ok(results);
    }

    [HttpPost("insert_auction")]
    public async Task<IActionResult> InsertAuction([FromBody] AuctionCreatorDto auctionCreatorDto)
    {
        var auction = new Auction
        {
            Auction_Id = Guid.NewGuid().ToString(),
            Title = auctionCreatorDto.Title,
            Description = auctionCreatorDto.Description,
            Starting_Price = auctionCreatorDto.Starting_Price,
            Current_Price = auctionCreatorDto.Starting_Price
        };

        HttpResponseMessage result = await _auctionService.InsertAuctionAsync(auction);

        if (!result.IsSuccessStatusCode)
        {
            return BadRequest(result.ReasonPhrase);
        }
        else
        {
            return Ok(auction);
        }
    }

    [HttpPost("insert_auction_bid")]
    public async Task<IActionResult> InsertAuctionBid([FromBody] AuctionBidDto auctionBidDto)
    {
        var auctionBid = new Auction_Bid
        {
            Bid_Id = Guid.NewGuid().ToString(),
            Auction_Id = auctionBidDto.Auction_Id,
            Username = auctionBidDto.Username,
            Bid_Amount = auctionBidDto.Bid_Amount,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        HttpResponseMessage result = await _auctionService.InsertBidAsync(auctionBid);

        if (!result.IsSuccessStatusCode)
        {
            string errorMessage = await result.Content.ReadAsStringAsync();
            return Ok(errorMessage);
        }
        else
        {
            return Ok(auctionBid);
        }
    }


    [HttpDelete("drop_tables")]
    public async Task<IActionResult> DropTables()
    {
        await _auctionService.DropTablesAsync();

        return Ok();
    }

    [HttpGet("get_all_auctions")]
    [ProducesResponseType(typeof(AuctionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAuctions([FromQuery] bool sortByDate = false)
    {
        var auctions = await _auctionService.GetAllAuctions();

        if (sortByDate)
        {
            auctions = Sorter.SortByDate(auctions, auction => auction.Created_At);
        }

        return Ok(auctions);
    }

    [HttpGet("get_auctions")]
    [ProducesResponseType(typeof(AuctionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuctions([FromQuery] int limit, [FromQuery] bool sortByDate = false)
    {
        var auctions = await _auctionService.GetAuctions(limit);

        if (sortByDate)
        {
            auctions = Sorter.SortByDate(auctions, auction => auction.Created_At);
        }

        return Ok(auctions);
    }

    [HttpGet("get_auction_by_id")]
    [ProducesResponseType(typeof(AuctionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuctionById(string auction_id)
    {
        var auction = await _auctionService.GetAuctionById(auction_id);

        return Ok(auction);
    }

    [HttpGet("get_all_bids")]
    [ProducesResponseType(typeof(AuctionBidDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBids()
    {
        var auctionBids = await _auctionService.GetAllBids();

        return Ok(auctionBids);
    }

    [HttpGet("get_bid_messages_for_auction")]
    public async Task<IActionResult> GetBidMessagesForAuction(string auction_Id)
    {
        var messages = await _auctionService.GetBidMessagesForAuction(auction_Id);

        return Ok(messages);
    }
}