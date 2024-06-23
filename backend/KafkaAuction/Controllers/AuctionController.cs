using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
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
    [ProducesResponseType(typeof(TablesResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTables()
    {
        var results = await _auctionService.CreateAuctionTableAsync();

        return Ok(results);
    }

    [HttpPost("create_streams")]
    [ProducesResponseType(typeof(StreamsResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateStreams()
    {
        var results = await _auctionService.CreateAuctionBidStreamAsync();

        return Ok(results);
    }

    [HttpPost("crate_auction_with_bids_stream")]
    [ProducesResponseType(typeof(StreamsResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAuctionWithBidsStream()
    {
        var results = await _auctionService.CreateAuctionsWithBidsStreamAsync();

        return Ok(results);
    }

    [HttpDelete("drop_tables")]
    [ProducesResponseType(typeof(DropResourceResponseDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> DropTables()
    {
        var results = await _auctionService.DropTablesAsync();

        return Ok(results);
    }

    [HttpPost("insert_auction")]
    [ProducesResponseType(typeof(AuctionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertAuction([FromBody] AuctionCreatorDto auctionCreatorDto)
    {
        if (string.IsNullOrEmpty(auctionCreatorDto.Title) || string.IsNullOrEmpty(auctionCreatorDto.Description) || auctionCreatorDto.Starting_Price == 0)
        {
            return BadRequest("Fields cannot be empty or zero.");
        }

        var auction = new Auction
        {
            Auction_Id = Guid.NewGuid().ToString(),
            Title = auctionCreatorDto.Title,
            Description = auctionCreatorDto.Description,
            Starting_Price = auctionCreatorDto.Starting_Price,
            Current_Price = auctionCreatorDto.Starting_Price,
            End_Date = DateTime.UtcNow.AddHours(auctionCreatorDto.Duration).ToString("yyyy-MM-dd HH:mm:ss")
        };

        var (httpResponseMessage, auctionDto) = await _auctionService.InsertAuctionAsync(auction);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return BadRequest(httpResponseMessage.ReasonPhrase);
        }
        else
        {
            return Ok(auctionDto);
        }
    }

    [HttpPatch("end_auction")]
    [ProducesResponseType(typeof(AuctionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> EndAuction(string auction_id)
    {
        var (httpResponseMessage, auctionDto) = await _auctionService.EndAuctionAsync(auction_id);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return BadRequest(httpResponseMessage.ReasonPhrase);
        }
        else
        {
            return Ok(auctionDto);
        }
    }

    [HttpDelete("delete_auction")]
    [ProducesResponseType(typeof(Auction), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAuction(string auction_id)
    {
        var (httpResponseMessage, auction) = await _auctionService.DeleteAuction(auction_id);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return BadRequest(httpResponseMessage.ReasonPhrase);
        }
        else
        {
            return Ok(auction);
        }
    }

    [HttpPost("insert_auction_bid")]
    [ProducesResponseType(typeof(AuctionBidCreatorDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertAuctionBid([FromBody] AuctionBidCreatorDto auctionBidDto)
    {
        var auctionBid = new Auction_Bid
        {
            Bid_Id = Guid.NewGuid().ToString(),
            Auction_Id = auctionBidDto.Auction_Id,
            Username = auctionBidDto.Username,
            Bid_Amount = auctionBidDto.Bid_Amount,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        var (httpResponseMessage, bidDto) = await _auctionService.InsertBidAsync(auctionBid);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            // TODO: Figure out why retrieving the error message from BadRequest response is not working
            return Ok(errorMessage);
        }
        else
        {
            return Ok(bidDto);
        }
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
        var auction = await _auctionService.GetAuctionDtoById(auction_id);

        return Ok(auction);
    }

    [HttpGet("get_all_bids")]
    [ProducesResponseType(typeof(AuctionBidCreatorDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBids()
    {
        var auctionBids = await _auctionService.GetAllBids();

        return Ok(auctionBids);
    }

    [HttpGet("get_all_auctions_with_bids")]
    [ProducesResponseType(typeof(AuctionWithBidDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAuctionsAndBids()
    {
        var auctionsWithBids = await _auctionService.GetAllAuctionWithBids();

        return Ok(auctionsWithBids);
    }

    [HttpGet("get_bid_messages_for_auction")]
    [ProducesResponseType(typeof(AuctionBidMessageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBidMessagesForAuction(string auction_Id)
    {
        var messages = await _auctionService.GetBidMessagesForAuction(auction_Id);

        return Ok(messages);
    }
}