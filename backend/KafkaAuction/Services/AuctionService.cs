
using KafkaAuction.Dtos;
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
    private readonly string _auctionsTableName = "auctions";
    private readonly string _auctionBidsStreamName = "auction_bids";
    private readonly string _auctionsWithBidsStreamName = "auctions_with_bids";

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
        await _restApiProvider.DropTableAndTopic(_auctionsTableName.ToUpper());
        await _restApiProvider.DropTableAndTopic("QUERYABLE_" + _auctionsTableName.ToUpper());
        await _restApiProvider.DropStreamAndTopic(_auctionBidsStreamName.ToUpper());
        await _restApiProvider.DropStreamAndTopic(_auctionsWithBidsStreamName.ToUpper());
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

    public async Task<List<AuctionDto>> GetAuctions(int limit)
    {
        var auctions = _context.CreatePullQuery<Auction>($"queryable_{_auctionsTableName}")
            .Take(limit);

        List<AuctionDto> auctionDtos = [];

        await foreach (var auction in auctions.GetManyAsync().ConfigureAwait(false))
        {
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

    public async Task<HttpResponseMessage> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default)
    {
        string createStreamSql = $@"
            CREATE OR REPLACE STREAM {_auctionsWithBidsStreamName} AS
                SELECT
                    a.Auction_Id,
                    a.Title,
                    b.Username,
                    b.Bid_Amount,
                    b.Bid_Time
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
            return response;
        }

        _logger.LogInformation($"{_auctionsWithBidsStreamName} stream created successfully.");
        return response;
    }
}
