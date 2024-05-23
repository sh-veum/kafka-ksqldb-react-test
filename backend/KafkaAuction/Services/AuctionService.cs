
using KafkaAuction.Dtos;
using KafkaAuction.Enums;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Services;

public class AuctionService : IAuctionService
{
    private readonly ILogger<AuctionService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly KSqlDBContext _context;
    private readonly string _ksqlDbUrl;
    private readonly string _auctionsTableName = "AUCTIONS";
    private readonly string _auctionBidsStreamName = "AUCTION_BIDS";
    private readonly string _auctionsWithBidsStreamName = "AUCTIONS_WITH_BIDS";

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

    public async Task<TablesResponse[]> CreateAuctionTableAsync(CancellationToken cancellationToken = default)
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

    public async Task<StreamsResponse[]> CreateAuctionBidStreamAsync(CancellationToken cancellationToken = default)
    {
        var auctionBidTableCreator = new StreamCreator<Auction_Bid>(_restApiProvider, _logger);

        if (!await auctionBidTableCreator.CreateStreamAsync(_auctionBidsStreamName, cancellationToken))
        {
            throw new InvalidOperationException("Failed to create stream");
        }

        // Return all streams
        return await _restApiProvider.GetStreamsAsync(cancellationToken);
    }

    public async Task<StreamsResponse[]> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default)
    {
        string createStreamSql = $@"
            CREATE OR REPLACE STREAM {_auctionsWithBidsStreamName} AS
                SELECT
                    a.Auction_Id,
                    a.Title,
                    b.Username,
                    b.Bid_Amount,
                    b.Timestamp
                FROM
                    Auction_Bids b
                LEFT JOIN
                    Auctions a
                ON b.Auction_Id = a.Auction_Id
                EMIT CHANGES;";

        var ksqlDbStatement = new KSqlDbStatement(createStreamSql);
        var response = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError($"Error creating {_auctionsWithBidsStreamName} stream: {content}");
        }

        _logger.LogInformation($"{_auctionsWithBidsStreamName} stream created successfully.");

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
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        await dropper.DropResourceAsync("QUERYABLE_" + _auctionsTableName, ResourceType.Table);
        await dropper.DropResourceAsync(_auctionsTableName, ResourceType.Table);
        await dropper.DropResourceAsync(_auctionBidsStreamName, ResourceType.Stream);
        await dropper.DropResourceAsync(_auctionsWithBidsStreamName, ResourceType.Stream);
    }

    public async Task<List<AuctionDtoWithId>> GetAllAuctions()
    {
        var auctions = _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .GetManyAsync();

        _logger.LogInformation("Found {amount} of auctions", await auctions.CountAsync());

        List<AuctionDtoWithId> auctionDtos = [];

        await foreach (var auction in auctions.ConfigureAwait(false))
        {
            auctionDtos.Add(new AuctionDtoWithId
            {
                Auction_Id = auction.Auction_Id,
                Title = auction.Title
            });
        }

        return auctionDtos;
    }

    public async Task<List<AuctionDtoWithId>> GetAuctions(int limit)
    {
        var auctions = _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .Take(limit);

        List<AuctionDtoWithId> auctionDtos = [];

        await foreach (var auction in auctions.GetManyAsync().ConfigureAwait(false))
        {
            auctionDtos.Add(new AuctionDtoWithId
            {
                Auction_Id = auction.Auction_Id,
                Title = auction.Title
            });
        }

        return auctionDtos;
    }

    public async Task<List<AuctionBidDtoWithTimeStamp>> GetAllBids()
    {
        var auctionBids = _context.CreatePullQuery<Auction_Bid>()
            .GetManyAsync();

        _logger.LogInformation("Found {amount} of auctions", await auctionBids.CountAsync());

        List<AuctionBidDtoWithTimeStamp> auctionBidDtos = [];

        await foreach (var auctionBid in auctionBids.ConfigureAwait(false))
        {
            auctionBidDtos.Add(new AuctionBidDtoWithTimeStamp
            {
                Auction_Id = auctionBid.Auction_Id,
                Username = auctionBid.Username,
                Bid_Amount = auctionBid.Bid_Amount,
                Timestamp = auctionBid.Timestamp
            });
        }

        return auctionBidDtos;
    }
}
