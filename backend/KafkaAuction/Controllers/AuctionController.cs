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
        var results = await _auctionService.CreateTablesAsync();

        if (results?.Count == 0 || results == null)
        {
            return Ok("Tables already exist or failed to create tables");
        }
        else
        {
            return Ok(results);
        }
    }

    [HttpPost("create_streams")]
    public async Task<IActionResult> CreateStreams()
    {
        var results = await _auctionService.CreateStreamsAsync();

        if (results?.Count == 0 || results == null)
        {
            return Ok("Tables already exist or failed to create streams");
        }
        else
        {
            return Ok(results);
        }
    }

    [HttpPost("insert_auction")]
    public async Task<IActionResult> InsertAuction(AuctionDto auctionDto)
    {
        var auction = new Auction
        {
            Auction_Id = auctionDto.Auction_Id,
            Title = auctionDto.Title
        };

        var result = await _auctionService.InsertAuctionAsync(auction);

        return Ok(result);
    }

    [HttpPost("insert_auction_bid")]
    public async Task<IActionResult> InsertAuctionBid(AuctionBidDto auctionBidDto)
    {
        var auctionBid = new Auction_Bid
        {
            Auction_Id = auctionBidDto.Auction_Id,
            Username = auctionBidDto.Username,
            Bid_Amount = auctionBidDto.Bid_Amount
        };

        var result = await _auctionService.InsertBidAsync(auctionBid);

        return Ok(result);
    }

    [HttpDelete("drop_tables")]
    public async Task<IActionResult> DropTables()
    {
        await _auctionService.DropTablesAsync();

        return Ok();
    }
}