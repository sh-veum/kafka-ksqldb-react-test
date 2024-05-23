using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
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
    public async Task<IActionResult> InsertAuction([FromBody] AuctionDto auctionDto)
    {
        var auction = new Auction
        {
            Auction_Id = Guid.NewGuid().ToString(),
            Title = auctionDto.Title
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
            return BadRequest(result.ReasonPhrase);
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
    public async Task<IActionResult> GetAllAuctions()
    {
        var auctions = await _auctionService.GetAllAuctions();

        return Ok(auctions);
    }

    [HttpGet("get_all_bids")]
    [ProducesResponseType(typeof(AuctionBidDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBids()
    {
        var auctionBids = await _auctionService.GetAllBids();

        return Ok(auctionBids);
    }

    [HttpGet("get_auctions")]
    [ProducesResponseType(typeof(AuctionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActions([FromQuery] int limit)
    {
        var auctions = await _auctionService.GetAuctions(limit);

        return Ok(auctions);
    }


}