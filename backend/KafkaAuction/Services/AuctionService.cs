using System.Net;
using KafkaAuction.Constants;
using KafkaAuction.Dtos;
using KafkaAuction.Enums;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services;

public class AuctionService : IAuctionService
{
    private readonly ILogger<AuctionService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly IServiceFactory _serviceFactory;
    private readonly KSqlDBContext _context;
    private readonly string _auctionsTableName = TableNameConstants.Auctions;
    private readonly string _auctionBidsStreamName = StreamNameConstants.AuctionBids;
    private readonly string _auctionsWithBidsStreamName = StreamNameConstants.AuctionWithBids;

    public AuctionService(ILogger<AuctionService> logger, IKSqlDbRestApiProvider restApiProvider, IServiceFactory serviceFactory, IConfiguration configuration)
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

    public async Task<TablesResponse[]> CreateAuctionTableAsync(CancellationToken cancellationToken = default)
    {
        var auctionTableCreator = new TableCreator<Auction>(_restApiProvider, _logger);
        if (!await auctionTableCreator.CreateTableAsync(_auctionsTableName, cancellationToken))
        {
            throw new InvalidOperationException($"Failed to create {_auctionsTableName} table");
        }

        // Create a queryable table of the auctions table
        await auctionTableCreator.CreateQueryableTableAsync(_auctionsTableName, cancellationToken);

        // Return all tables
        return await _restApiProvider.GetTablesAsync(cancellationToken);
    }

    public async Task<(HttpResponseMessage httpResponseMessage, AuctionBidDto? auctionBidDto)> InsertBidAsync(Auction_Bid auctionBid)
    {
        var auction = await GetAuctionByIdAsync(auctionBid.Auction_Id);

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

    public async Task<(HttpResponseMessage httpResponseMessage, AuctionDto? auctionDto)> InsertAuctionAsync(Auction auction)
    {
        var inserter = new EntityInserter<Auction>(_restApiProvider, _logger);
        var insertResponse = await inserter.InsertAsync(_auctionsTableName, auction);

        if (!insertResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to insert AUCTION: {StatusCode} {ReasonPhrase}", insertResponse.StatusCode, insertResponse.ReasonPhrase);
            return (insertResponse, null);
        }

        var auctionDto = new AuctionDto
        {
            Auction_Id = auction.Auction_Id,
            Title = auction.Title,
            Description = auction.Description,
            Starting_Price = auction.Starting_Price,
            Current_Price = auction.Current_Price,
            Leader = auction.Leader,
            Number_Of_Bids = auction.Number_Of_Bids,
            Winner = auction.Winner,
            Created_At = auction.Created_At,
            End_Date = auction.End_Date,
            Is_Open = auction.Is_Open
        };

        return (insertResponse, auctionDto);
    }

    public async Task<List<DropResourceResponseDto>> DropAuctionTablesAsync()
    {
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        var resourcesToDrop = new List<(string Name, ResourceType Type)>
        {
            ("QUERYABLE_" + _auctionsTableName, ResourceType.Table),
            (_auctionsWithBidsStreamName, ResourceType.Stream),
            (_auctionsTableName, ResourceType.Table),
            (_auctionBidsStreamName, ResourceType.Stream)
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


    public async Task<List<AuctionDto>> GetAllAuctions()
    {
        var auctions = _context.CreatePullQuery<Auction>($"QUERYABLE_{_auctionsTableName}")
            .Where(a => a.Is_Existing == true)
            .GetManyAsync();

        // _logger.LogInformation("Found {amount} auctions", await auctions.CountAsync());

        List<AuctionDto> auctionDtos = [];

        await foreach (var auction in auctions.ConfigureAwait(false))
        {
            auctionDtos.Add(new AuctionDto
            {
                Auction_Id = auction.Auction_Id,
                Title = auction.Title,
                Description = auction.Description,
                Number_Of_Bids = auction.Number_Of_Bids,
                Starting_Price = auction.Starting_Price,
                Current_Price = auction.Current_Price,
                Leader = auction.Leader,
                Winner = auction.Winner,
                Created_At = auction.Created_At,
                End_Date = auction.End_Date,
                Is_Open = auction.Is_Open
            });
        }

        return auctionDtos;
    }

    /// <summary>
    /// Since the pull query is not sorted, this is mostly useless.
    /// </summary>
    /// <param name="limit">Amount of auctions to pull</param>
    /// <returns>A list of AuctionDtos</returns>
    public async Task<List<AuctionDto>> GetAuctionsAsync(int limit)
    {
        var auctions = _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .Where(a => a.Is_Existing == true)
            .Take(limit)
            .GetManyAsync();

        List<AuctionDto> auctionDtos = [];

        await foreach (var auction in auctions.ConfigureAwait(false))
        {
            auctionDtos.Add(new AuctionDto
            {
                Auction_Id = auction.Auction_Id,
                Title = auction.Title,
                Description = auction.Description,
                Starting_Price = auction.Starting_Price,
                Current_Price = auction.Current_Price,
                Number_Of_Bids = auction.Number_Of_Bids,
                Leader = auction.Leader,
                Winner = auction.Winner,
                Created_At = auction.Created_At,
                End_Date = auction.End_Date,
                Is_Open = auction.Is_Open
            });
        }

        return auctionDtos;
    }

    public async Task<AuctionDto?> GetAuctionDtoByIdAsync(string auction_id)
    {
        var auction = await _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .Where(a => a.Auction_Id == auction_id && a.Is_Existing == true)
            .FirstOrDefaultAsync();

        if (auction == null)
        {
            return null;
        }

        return new AuctionDto
        {
            Auction_Id = auction.Auction_Id,
            Title = auction.Title,
            Description = auction.Description,
            Starting_Price = auction.Starting_Price,
            Current_Price = auction.Current_Price,
            Leader = auction.Leader,
            Number_Of_Bids = auction.Number_Of_Bids,
            Winner = auction.Winner,
            Created_At = auction.Created_At,
            End_Date = auction.End_Date,
            Is_Open = auction.Is_Open
        };
    }

    public async Task<Auction?> GetAuctionByIdAsync(string auction_id)
    {
        var auction = await _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .Where(a => a.Auction_Id == auction_id && a.Is_Existing == true)
            .FirstOrDefaultAsync();

        if (auction == null)
        {
            _logger.LogWarning($"Auction with id {auction_id} not found");
            return null;
        }

        return auction;
    }

    public async Task<(HttpResponseMessage httpResponseMessage, Auction? auction)> DeleteAuction(string auction_id)
    {
        var auction = await GetAuctionByIdAsync(auction_id);

        if (auction == null)
        {
            return (new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Auction to delete not found")
            }, null);
        }

        auction.Is_Existing = false;

        var auctionInserter = new EntityInserter<Auction>(_restApiProvider, _logger);
        var response = await auctionInserter.InsertAsync(_auctionsTableName, auction);

        return (response, auction);
    }

    public async Task<(HttpResponseMessage httpResponseMessage, AuctionDto? auctionDto)> EndAuctionAsync(string auction_id)
    {
        var auction = await GetAuctionByIdAsync(auction_id);

        if (auction == null)
        {
            return (new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Auction to end not found")
            }, null);
        }

        auction.Is_Open = false;

        var auctionBidService = _serviceFactory.CreateAuctionBidService();

        var auctionBids = await auctionBidService.GetBidsForAuctionAsync(auction_id);

        if (auctionBids.Count == 0)
        {
            auction.Winner = "No bids";
        }
        else
        {
            var winningBid = auctionBids.OrderByDescending(b => b.Bid_Amount).First();
            auction.Winner = winningBid.Username;
        }

        var auctionInserter = new EntityInserter<Auction>(_restApiProvider, _logger);
        var response = await auctionInserter.InsertAsync(_auctionsTableName, auction);

        var auctionDto = new AuctionDto
        {
            Auction_Id = auction.Auction_Id,
            Title = auction.Title,
            Description = auction.Description,
            Starting_Price = auction.Starting_Price,
            Current_Price = auction.Current_Price,
            Leader = auction.Leader,
            Number_Of_Bids = auction.Number_Of_Bids,
            Winner = auction.Winner,
            Created_At = auction.Created_At,
            End_Date = auction.End_Date,
            Is_Open = auction.Is_Open
        };

        return (response, auctionDto);
    }
}
