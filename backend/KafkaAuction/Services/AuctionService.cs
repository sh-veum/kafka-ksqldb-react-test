
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Properties;

namespace KafkaAuction.Services;

public class AuctionService : IAuctionService
{
    private readonly ILogger<AuctionService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly string _auctionsTableName = "auctions";
    private readonly string _auctionBidsStreamName = "auction_bids";

    public AuctionService(ILogger<AuctionService> logger, IKSqlDbRestApiProvider restApiProvider)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;
    }

    public async Task<List<string>?> CreateTablesAsync(CancellationToken cancellationToken = default)
    {
        var createdTables = new List<string>();

        var auctionTableCreator = new TableCreator<Auction>(_restApiProvider, _logger);
        if (await auctionTableCreator.CreateTableAsync(_auctionsTableName, cancellationToken))
        {
            createdTables.Add(_auctionsTableName);
        }

        return createdTables;
    }

    public async Task<List<string>?> CreateStreamsAsync(CancellationToken cancellationToken = default)
    {
        var createdStreams = new List<string>();

        var auctionBidTableCreator = new StreamCreator<Auction_Bid>(_restApiProvider, _logger);
        if (await auctionBidTableCreator.CreateStreamAsync(_auctionBidsStreamName, cancellationToken))
        {
            createdStreams.Add(_auctionBidsStreamName);
        }

        return createdStreams;
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
}
