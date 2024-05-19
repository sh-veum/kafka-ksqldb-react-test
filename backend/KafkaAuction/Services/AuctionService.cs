
using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Properties;

namespace KafkaAuction.Services;

public class AuctionService : IAuctionService
{
    private readonly ILogger<AuctionService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly KSqlDBContext _context;
    private readonly string _ksqlDbUrl;
    private readonly string _auctionsTableName = "auctions";
    private readonly string _auctionBidsStreamName = "auction_bids";

    public AuctionService(ILogger<AuctionService> logger, IKSqlDbRestApiProvider restApiProvider, IConfiguration configuration)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;

        _ksqlDbUrl = configuration.GetValue<string>("KSqlDb:Url") ?? string.Empty;
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

    public async Task<TablesResponse[]> CreateTablesAsync(CancellationToken cancellationToken = default)
    {
        var auctionTableCreator = new TableCreator<Auction>(_restApiProvider, _logger);
        if (!await auctionTableCreator.CreateTableAsync(_auctionsTableName, cancellationToken))
        {
            throw new InvalidOperationException("Failed to create table");

        }

        // Create a queryable table of the auctions table
        await auctionTableCreator.CreateQueryableTableAsync(_auctionsTableName, cancellationToken);

        // Return all tables
        return await _restApiProvider.GetTablesAsync(cancellationToken);
    }

    public async Task<StreamsResponse[]> CreateStreamsAsync(CancellationToken cancellationToken = default)
    {
        var auctionBidTableCreator = new StreamCreator<Auction_Bid>(_restApiProvider, _logger);

        if (!await auctionBidTableCreator.CreateStreamAsync(_auctionBidsStreamName, cancellationToken))
        {
            throw new InvalidOperationException("Failed to create stream");
        }

        // Return all streams
        return await _restApiProvider.GetStreamsAsync(cancellationToken);
    }

    public async Task<HttpResponseMessage> InsertBidAsync(Auction_Bid auctionBid)
    {
        var inserter = new EntityInserter<Auction_Bid>(_restApiProvider, _logger);
        return await inserter.InsertAsync(_auctionBidsStreamName, auctionBid);
    }

    public async Task<HttpResponseMessage> InsertAuctionAsync(Auction auction)
    {
        var inserter = new EntityInserter<Auction>(_restApiProvider, _logger);
        return await inserter.InsertAsync(_auctionsTableName, auction);
    }

    public async Task DropTablesAsync()
    {
        await _restApiProvider.DropTableAndTopic(_auctionsTableName);
        await _restApiProvider.DropStreamAndTopic(_auctionBidsStreamName);
    }

    public async Task<bool> DropSingleTableAsync(string tableName)
    {
        var httpResult = await _restApiProvider.DropTableAndTopic(tableName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
            return false;
        }

        return true;
    }

    public async Task<List<AuctionDto>> GetAllAuctions()
    {
        var auctions = _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .GetManyAsync();

        _logger.LogInformation("GetAllAuctions: {Auctions}", auctions);

        List<AuctionDto> auctionDtos = [];

        await foreach (var auction in auctions.ConfigureAwait(false))
        {
            _logger.LogInformation("GetAllAuctions: {Auction}", auction.ToString());
            auctionDtos.Add(new AuctionDto
            {
                Auction_Id = auction.Auction_Id,
                Title = auction.Title
            });
        }

        return auctionDtos;
    }

    public async Task<List<AuctionBidDto>> GetAllBids()
    {
        var auctionBids = _context.CreatePullQuery<Auction_Bid>()
            .GetManyAsync();

        List<AuctionBidDto> auctionBidDtos = [];

        await foreach (var auctionBid in auctionBids.ConfigureAwait(false))
        {
            _logger.LogInformation("GetAllBids: {auctionBid}", auctionBid.ToString());
            auctionBidDtos.Add(new AuctionBidDto
            {
                Auction_Id = auctionBid.Auction_Id,
                Username = auctionBid.Username,
                Bid_Amount = auctionBid.Bid_Amount
            });
        }

        return auctionBidDtos;
    }

    // public async Task<List<AuctionDto>> GetAuctions(int limit)
    // {
    //     var auctions = _context.CreatePullQuery<Auction>()
    //         .Take(limit);

    //     List<AuctionDto> auctionDtos = [];

    //     await foreach (var auction in auctions)
    //     {
    //         auctionDtos.Add(new AuctionDto
    //         {
    //             Auction_Id = auction.Auction_Id,
    //             Title = auction.Title
    //         });
    //     }

    //     return auctionDtos;
    // }
}
