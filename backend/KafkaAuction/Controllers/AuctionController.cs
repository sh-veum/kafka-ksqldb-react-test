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

    [HttpPost("CreateTables")]
    public async Task<IActionResult> CreateTables()
    {
        var restult = await _auctionService.CreateTablesAsync();

        return Ok(restult);
    }

    [HttpPost("InsertAuction")]
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

    [HttpPost("InsertAuctionBid")]
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

    [HttpDelete("DropTables")]
    public async Task<IActionResult> DropTables()
    {
        await _auctionService.DropTablesAsync();

        return Ok();
    }
}