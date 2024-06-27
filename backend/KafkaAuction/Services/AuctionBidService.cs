using System.Net;
using KafkaAuction.Constants;
using KafkaAuction.Dtos;
using KafkaAuction.Enums;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;

namespace KafkaAuction.Services;

public class AuctionBidService : IAuctionBidService
{
    private readonly ILogger<AuctionBidService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly IServiceFactory _serviceFactory;
    private readonly KSqlDBContext _context;
    private readonly string _auctionsTableName = TableNameConstants.Auctions;
    private readonly string _auctionBidsStreamName = StreamNameConstants.AuctionBids;

    public AuctionBidService(ILogger<AuctionBidService> logger, IKSqlDbRestApiProvider restApiProvider, IServiceFactory serviceFactory, IConfiguration configuration)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;
        _serviceFactory = serviceFactory;

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

    public async Task<StreamsResponse[]> CreateAuctionBidStreamAsync(CancellationToken cancellationToken = default)
    {
        var auctionBidTableCreator = new StreamCreator<Auction_Bid>(_restApiProvider, _logger);

        if (!await auctionBidTableCreator.CreateStreamAsync(_auctionBidsStreamName, cancellationToken))
        {
            throw new InvalidOperationException($"Failed to create {_auctionBidsStreamName} stream");
        }

        // Return all streams
        return await _restApiProvider.GetStreamsAsync(cancellationToken);
    }

    public async Task<(HttpResponseMessage httpResponseMessage, AuctionBidDto? auctionBidDto)> InsertBidAsync(Auction_Bid auctionBid)
    {
        var auctionService = _serviceFactory.CreateAuctionService();

        var auction = await auctionService.GetAuctionByIdAsync(auctionBid.Auction_Id);

        if (auction == null)
        {
            return (new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Auction to insert bid in not found")
            }, null);
        }

        if (DateTime.Parse(auction.End_Date) < DateTime.UtcNow)
        {
            return (new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Auction has ended")
            }, null);
        }

        if (!auction.Is_Open || !auction.Is_Existing)
        {
            return (new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Auction is closed")
            }, null);
        }

        _logger.LogInformation("Current price: {CurrentPrice}", auction.Current_Price);
        _logger.LogInformation("Bid amount: {BidAmount}", auctionBid.Bid_Amount);

        if (auctionBid.Bid_Amount > auction.Current_Price)
        {
            auction.Current_Price = auctionBid.Bid_Amount;
            auction.Leader = auctionBid.Username;
            auction.Number_Of_Bids += 1;

            var auctionInserter = new EntityInserter<Auction>(_restApiProvider, _logger);
            var insertAuctionResponse = await auctionInserter.InsertAsync(_auctionsTableName, auction);

            if (!insertAuctionResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to insert AUCTION: {StatusCode} {ReasonPhrase}", insertAuctionResponse.StatusCode, insertAuctionResponse.ReasonPhrase);
                return (insertAuctionResponse, null);
            }

            var inserter = new EntityInserter<Auction_Bid>(_restApiProvider, _logger);
            var insertBidResponse = await inserter.InsertAsync(_auctionBidsStreamName, auctionBid);

            if (!insertBidResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to insert AUCTION_BID: {StatusCode} {ReasonPhrase}", insertBidResponse.StatusCode, insertBidResponse.ReasonPhrase);
                return (insertBidResponse, null);
            }
            else
            {
                var auctionBidDto = new AuctionBidDto
                {
                    Auction_Id = auctionBid.Auction_Id,
                    Username = auctionBid.Username,
                    Bid_Amount = auctionBid.Bid_Amount,
                    Timestamp = auctionBid.Timestamp
                };

                return (new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Bid inserted successfully")
                }, auctionBidDto);
            }
        }
        else
        {
            _logger.LogInformation("Bid is lower than current price");
            return (new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent($"Bid must be higher than: {auction.Current_Price}")
            }, null);
        }
    }

    public async Task<DropResourceResponseDto> DropBidStreamAsync()
    {
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        var response = await dropper.DropResourceAsync(_auctionBidsStreamName, ResourceType.Stream);

        var dropResourceResponseDto = new DropResourceResponseDto
        {
            ResourceName = _auctionBidsStreamName,
            IsSuccess = response.IsSuccessStatusCode
        };

        return dropResourceResponseDto;
    }

    public async Task<List<AuctionBidDto>> GetAllBidsAsync()
    {
        var auctionBids = _context.CreatePullQuery<Auction_Bid>()
            .GetManyAsync();

        List<AuctionBidDto> auctionBidDtos = [];

        await foreach (var auctionBid in auctionBids.ConfigureAwait(false))
        {
            auctionBidDtos.Add(new AuctionBidDto
            {
                Auction_Id = auctionBid.Auction_Id,
                Username = auctionBid.Username,
                Bid_Amount = auctionBid.Bid_Amount,
                Timestamp = auctionBid.Timestamp
            });
        }

        return auctionBidDtos;
    }

    /// <summary>
    /// Returns a list of bids for an auction
    /// Contains more detailed information about the bids than GetBidMessagesForAuction
    /// </summary>
    /// <param name="auction_id">Auction to get messages from</param>
    /// <returns>A list of AuctionBidDtos</returns>
    public async Task<List<AuctionBidDto>> GetBidsForAuctionAsync(string auction_id)
    {
        var auctionBids = _context.CreatePullQuery<Auction_Bid>()
            .Where(a => a.Auction_Id == auction_id);

        List<AuctionBidDto> auctionBidDtos = [];

        await foreach (var auctionBid in auctionBids.GetManyAsync().ConfigureAwait(false))
        {
            auctionBidDtos.Add(new AuctionBidDto
            {
                Auction_Id = auctionBid.Auction_Id,
                Username = auctionBid.Username,
                Bid_Amount = auctionBid.Bid_Amount,
                Timestamp = auctionBid.Timestamp
            });
        }

        return auctionBidDtos;
    }

    /// <summary>
    /// Returns a list of bid messages for an auction
    /// Contains only necessary information for a bids table
    /// </summary>
    /// <param name="auction_id">Auction to get messages from</param>
    /// <returns>A list of AuctionBidMessageDtos</returns>
    public async Task<List<AuctionBidMessageDto>> GetBidMessagesForAuctionAsync(string auction_id)
    {
        var auctionBids = _context.CreatePullQuery<Auction_Bid>()
            .Where(a => a.Auction_Id == auction_id);

        List<AuctionBidMessageDto> auctionBidDtos = [];

        await foreach (var auctionBid in auctionBids.GetManyAsync().ConfigureAwait(false))
        {
            auctionBidDtos.Add(new AuctionBidMessageDto
            {
                Username = auctionBid.Username,
                Bid_Amount = auctionBid.Bid_Amount,
                Timestamp = auctionBid.Timestamp
            });
        }

        return auctionBidDtos;
    }
}
